using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace M2SA.AppGenome.Configuration
{
    /// <summary>
    /// 
    /// </summary>
    public interface IConfigNode
    {
        /// <summary>
        /// 节点名称
        /// </summary>
        string Name
        {
            get;
        }

        /// <summary>
        /// 节点类型
        /// </summary>
        string MetaType
        {
            get;
            set;
        }

        /// <summary>
        /// 启用单例
        /// </summary>
        bool EnableSingleton
        {
            get;
            set;
        }

        int PropertiesCount
        {
            get;
        }

        /// <summary>
        /// 获取所有属性值
        /// </summary>
        /// <returns></returns>
        IDictionary<string, object> GetProperties();

        /// <summary>
        /// 是否存在属性值
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        bool ContainsProperty(string propertyName);

        /// <summary>
        /// 获取属性值
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        object GetProperty(string propertyName);

        /// <summary>
        /// 获取属性值
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        object GetProperty(string propertyName, object defaultValue);

        /// <summary>
        /// 获取属性值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        T GetProperty<T>(string propertyName);

        /// <summary>
        /// 获取属性值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="propertyName"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        T GetProperty<T>(string propertyName, T defaultValue);

        /// <summary>
        /// 设置属性值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="propertyName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        void SetProperty<T>(string propertyName, T value);

        /// <summary>
        /// 获取子节点列表
        /// </summary>
        /// <param name="nodeName"></param>
        /// <returns></returns>
        IList<IConfigNode> GetNodeList(string nodeName);

        /// <summary>
        /// 获取子节点字典
        /// </summary>
        /// <param name="nodeName"></param>
        /// <returns></returns>
        IDictionary<string, IConfigNode> GetNodeMap(string nodeName);
    }
}
