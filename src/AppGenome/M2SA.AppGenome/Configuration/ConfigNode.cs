using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;
using M2SA.AppGenome.Reflection;

namespace M2SA.AppGenome.Configuration
{
    /// <summary>
    /// 
    /// </summary>
    public class ConfigNode : IConfigNode
    {
        private static readonly string MetaTypeName = "c:type";
        private static readonly string EnableSingletonName = "c:enableSingleton";

        IDictionary<string, object> propertyMap = null;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configNode"></param>        
        public ConfigNode(XmlNode configNode)
        {
            if (null == configNode)
                throw new ArgumentNullException("configNode");

            this.Name = configNode.Name.ToLower();

            this.ResolveObject(configNode);
            if (this.ContainsProperty(MetaTypeName))
            {
                this.MetaType = this.GetProperty<string>(MetaTypeName);
            }
            if (this.ContainsProperty(EnableSingletonName))
            {
                this.EnableSingleton = this.GetProperty<bool>(EnableSingletonName);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>        
        protected void ResolveObject(XmlNode node)
        {
            if (null == node)
                throw new ArgumentNullException("node");

            this.propertyMap = new Dictionary<string, object>(node.Attributes.Count + 4);

            foreach (XmlAttribute attribute in node.Attributes)
            {
                this.propertyMap[attribute.Name.ToLower()] = attribute.InnerText;
            }            

            foreach (XmlNode childNode in node.ChildNodes)
            {
                if (childNode.NodeType != XmlNodeType.Element)
                    continue;               

                if (node.SelectNodes(childNode.Name).Count > 1)
                {
                    var name = childNode.Name.ToLower();
                    AddChildNodeToProperty(new ConfigNode(childNode), name);
                    continue;
                }
                if (childNode.Attributes.Count == 0)
                {
                    if (childNode.ChildNodes.Count == 0)
                        continue;
                    if (childNode.ChildNodes.Count == 1 && childNode.ChildNodes[0].NodeType == XmlNodeType.Text)
                    {
                        this.propertyMap[childNode.Name.ToLower()] = childNode.InnerText;
                        continue;
                    }

                    if (childNode.ChildNodes.Count > 0 && childNode.ChildNodes.Count == childNode.SelectNodes(childNode.ChildNodes[0].Name).Count)
                    {
                        var name = childNode.Name.ToLower();
                        foreach (XmlNode item in childNode.ChildNodes)
                        {
                            AddChildNodeToProperty(new ConfigNode(item), name);
                        }
                        continue;
                    }
                }                

                this.propertyMap[childNode.Name.ToLower()] = new ConfigNode(childNode);
            }
        }

        void AddChildNodeToProperty(IConfigNode node, string propName)
        {
            IList<IConfigNode> list = null;
            if (this.propertyMap.ContainsKey(propName) == false)
            {
                list = new List<IConfigNode>(4);
                this.propertyMap.Add(propName, list);
            }
            else
            {
                list = (IList<IConfigNode>)this.propertyMap[propName];
            }
            list.Add(node);
        }

        #region IConfigNode 成员

        /// <summary>
        /// 
        /// </summary>
        public string Name
        {
            get;
            private set;
        }

        /// <summary>
        /// 
        /// </summary>
        public string MetaType
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public bool EnableSingleton
        {
            get;
            set;
        }

        /// <summary>
        /// 获取所有属性值
        /// </summary>
        /// <returns></returns>
        public IDictionary<string, object> GetProperties()
        {
            return this.propertyMap;
        }

        /// <summary>
        /// 是否存在属性值
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>        
        public bool ContainsProperty(string propertyName)
        {
            if (null == propertyName)
                throw new ArgumentNullException("propertyName");
            return this.propertyMap.ContainsKey(propertyName.ToLower());
        }

        /// <summary>
        /// 获取属性值
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public object GetProperty(string propertyName)
        {
            if (null == propertyName)
                throw new ArgumentNullException("propertyName");
            object result = null;
            if (this.propertyMap.ContainsKey(propertyName.ToLower()))
            {
                result = this.propertyMap[propertyName.ToLower()];
            }
            return result;
        }

        /// <summary>
        /// 获取属性值
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public object GetProperty(string propertyName, object defaultValue)
        {
            if (null == propertyName)
                throw new ArgumentNullException("propertyName");

            var result = GetProperty(propertyName.ToLower());
            if (null == result)
            {
                result = defaultValue;
            }
            return result;
        }

        /// <summary>
        /// 获取属性值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public T GetProperty<T>(string propertyName)
        {
            return GetProperty<T>(propertyName,default(T)) ;
        }

        /// <summary>
        /// 获取属性值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="propertyName"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public T GetProperty<T>(string propertyName, T defaultValue)
        {
            if (null == propertyName)
                throw new ArgumentNullException("propertyName");

            T result = defaultValue;
            if (this.propertyMap.ContainsKey(propertyName.ToLower()))
            {
                var val = this.propertyMap[propertyName.ToLower()];
                result = val.Convert<T>();
            }
            return result;
        }

        /// <summary>
        /// 设置属性值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="propertyName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public void SetProperty<T>(string propertyName, T value)
        {
            if (null == propertyName)
                throw new ArgumentNullException("propertyName");
            this.propertyMap[propertyName.ToLower()] = value;
        }

        /// <summary>
        /// 获取子节点列表
        /// </summary>
        /// <param name="nodeName"></param>
        /// <returns></returns>
        public IList<IConfigNode> GetNodeList(string nodeName)
        {
            if (null == nodeName)
                throw new ArgumentNullException("nodeName");
            var propValue = this.propertyMap[nodeName.ToLower()];
            return propValue as IList<IConfigNode>;
        }

        /// <summary>
        /// 获取子节点字典
        /// </summary>
        /// <param name="nodeName"></param>
        /// <returns></returns>
        public IDictionary<string, IConfigNode> GetNodeMap(string nodeName)
        {
            if (null == nodeName)
                throw new ArgumentNullException("nodeName");
            var nodeMap = new Dictionary<string, IConfigNode>();
            var propValue = this.propertyMap[nodeName.ToLower()];

            IList<IConfigNode> nodeList = null;
            if (propValue is IList)
            {
                nodeList = (IList<IConfigNode>)propValue;
            }
            else if (propValue is IConfigNode)
            {
                nodeList = new List<IConfigNode>(1) { (IConfigNode)propValue };
            }            

            if (nodeList != null && nodeList.Count > 0)
            {
                var node = nodeList[0];
                var strongKey = AppConfig.SrongNameSequence.FirstOrDefault<string>(
                    name => node.ContainsProperty(name));

                if (string.IsNullOrEmpty(strongKey))
                {
                    throw new ArgumentOutOfRangeException("nodeName", string.Format("not find the {0} key from {1}", node.Name, AppConfig.SrongNameSequence));
                }

                foreach (var configNode in nodeList)
                {
                    nodeMap.Add(configNode.GetProperty<string>(strongKey), configNode);
                }
            }
            return nodeMap;
        } 

        #endregion
    }
}
