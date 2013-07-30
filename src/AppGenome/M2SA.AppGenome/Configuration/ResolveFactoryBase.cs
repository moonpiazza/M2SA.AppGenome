using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;
using M2SA.AppGenome.Configuration;
using M2SA.AppGenome.Reflection;

namespace M2SA.AppGenome.Configuration
{
    /// <summary>
    /// IResolveObject对象工厂
    /// </summary>
    /// <typeparam name="TType"></typeparam>
    public abstract class ResolveFactoryBase<TType> where TType : IResolveObject
    {
        /// <summary>
        /// 
        /// </summary>
        protected static readonly object SyscObject = new object();

        static Type instanceType = typeof(TType);


        private IDictionary<string, TType> objectMap = new Dictionary<string, TType>(4);

        /// <summary>
        /// 
        /// </summary>
        protected IDictionary<string, TType> ObjectMap
        {
            get { return this.objectMap; }
        }

        /// <summary>
        /// 
        /// </summary>
        protected abstract string ModuleKey
        {
            get;
        }


        /// <summary>
        /// 获取默认名称的实例
        /// </summary>
        /// <returns></returns>
        public virtual TType GetInstance()
        {
            return this.GetInstance(null);
        }
        /// <summary>
        /// 获取强名称的实例
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public TType GetInstance(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                name = this.GetDefaultCategory();
            }

            TType obj = default(TType);
            if (this.ObjectMap.ContainsKey(name))
            {
                obj = this.ObjectMap[name];
            }
            else
            {
                lock (SyscObject)
                {
                    if (this.ObjectMap.ContainsKey(name))
                    {
                        obj = this.ObjectMap[name];
                    }
                    else
                    {
                        obj = this.ResolveObject(name);
                        if (null != obj)
                        {
                            this.ObjectMap[name] = obj;
                        }
                    }
                }
            }

            return obj;
        }

        /// <summary>
        /// 是否存在指定实例
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool Exists(string name)
        {
            return this.ObjectMap.ContainsKey(name);
        }

        /// <summary>
        /// 加载指定的配置信息
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        protected abstract IConfigNode LoadConfigInfo(string name);

        /// <summary>
        /// 创建对象实例
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        protected virtual TType ResolveObject(string name)
        {
            TType obj = default(TType);
            Type implementType = null;

            var config = this.LoadConfigInfo(name);
            if (null == config)
            {
                throw new ConfigException(string.Format("not find the config : {0}", name));
            }

            if (string.IsNullOrEmpty(config.MetaType))
            {
                implementType = instanceType.GetMapType();
            }
            else
            {
                implementType = TypeExtension.GetMapType(config.MetaType);
            }

            if (implementType == null)
            {
                throw new NotImplementedException(string.Format("cannot find the type : {0}[{1}] !", name, config.MetaType));
            }
            
            if (implementType.CanCreated())
            {
                obj = (TType)Activator.CreateInstance(implementType, false);
            }
            else
                throw new NotImplementedException(string.Format("{0} cannot be created !", implementType));

            if (null != obj)
            {
                obj.Initialize(config);
            }
            return obj;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected virtual string GetDefaultCategory()
        {
            var result = string.Empty;
            var configNode = AppInstance.GetConfigNode(this.ModuleKey, null);
            if (null != configNode)
                result = configNode.GetProperty<string>(AppConfig.DefaultKey);

            if (string.IsNullOrEmpty(result))
                result = AppConfig.DefaultKey;
            return result; 
        }
    }
}
