using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;

namespace M2SA.AppGenome.Cache
{
    /// <summary>
    /// 缓存监听接口
    /// </summary>
    public interface ICacheNotify
    {
        /// <summary>
        /// 根据配置节点初始化
        /// </summary>
        bool NeedItemHandler
        {
            get;
        }

        /// <summary>
        /// 关联cache实例
        /// </summary>
        /// <param name="cache"></param>
        void SetCache(ICache cache);

        /// <summary>
        /// Get数据前的执行方法
        /// </summary>
        /// <param name="key"></param>
        void OnBeforeGet(string key);

        /// <summary>
        /// Get数据后的执行方法
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        void OnAfterGet(string key, ref object value);

        /// <summary>
        /// Set数据前的执行方法
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        void OnBeforeSet(string key, object value);

        /// <summary>
        /// Set数据后的执行方法
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        void OnAfterSet(string key, object value);

        /// <summary>
        /// Remove数据前的执行方法
        /// </summary>
        /// <param name="key"></param>
        void OnBeforeRemove(string key);

        /// <summary>
        /// Remove数据前的执行方法
        /// </summary>
        /// <param name="key"></param>
        void OnAfterRemove(string key);
    }
}
