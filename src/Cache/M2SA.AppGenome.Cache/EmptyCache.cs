using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Management;
using M2SA.AppGenome.Reflection;

namespace M2SA.AppGenome.Cache
{
    /// <summary>
    /// 
    /// </summary>
    public class EmptyCache : AbstractCache
    {
        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public override object GetItem(string key)
        {
            return null;
        }

        /// <summary>
        /// 保存数据
        /// </summary>
        /// <param name="key"></param>
        /// <param name="data"></param>
        /// <param name="expiryTime"></param>
        public override void SetItem(string key, object data, TimeSpan expiryTime) { /* empty action */ }

        /// <summary>
        /// 移除数据
        /// </summary>
        /// <param name="key"></param>
        public override void RemoveItem(string key) { /* empty action */ }

        /// <summary>
        /// 清除缓存
        /// </summary>
        public override void Clear()
        {
            //Not Supported;
        }

        /// <summary>
        /// 获取缓存状态信息
        /// </summary>
        /// <returns></returns>
        public override object GetState()
        {
            return this;
        }
    }
}
