using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;
using M2SA.AppGenome.Reflection;
using M2SA.AppGenome.Logging;
using M2SA.AppGenome.Configuration;
using M2SA.AppGenome.Cache.MemCached;

namespace M2SA.AppGenome.Cache.MemCached
{
    /// <summary>
    /// 
    /// </summary>
    public class MemCache : AbstractCache
    {
        MemcachedProxy readProxy;
        MemcachedProxy writeProxy;

        /// <summary>
        /// 
        /// </summary>
        public IList<ServerNode> Servers
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public MemCache()
        {
            this.CompressionThreshold = 1024;
        }

        #region IResolveObject Members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="config"></param>
        public override void Initialize(IConfigNode config)
        {
            this.Servers = new List<ServerNode>(2);
            base.Initialize(config);

            if (this.Servers.Count == 0)
            {
                throw new ArgumentNullException("servers");
            }

            if (this.Notify != null && this.Notify.NeedItemHandler == true && this.LoadDataHandler == null)
            {
                throw new ConfigException("ActiveCache need to LoadDataHandler");
            }

            var readServers = new List<ServerNode>(this.Servers.Count);
            var writeServers = new List<ServerNode>(this.Servers.Count);
            foreach (var item in this.Servers)
            {
                if ((item.Scope & ServerScope.Read) == ServerScope.Read)
                    readServers.Add(item);
                if ((item.Scope & ServerScope.Write) == ServerScope.Write)
                    writeServers.Add(item);
            }
            this.readProxy = new MemcachedProxy(this.Name, ServerScope.Read, readServers);
            this.writeProxy = new MemcachedProxy(this.Name, ServerScope.Write, writeServers);
            this.writeProxy.CompressionThreshold = this.CompressionThreshold;

            LogManager.GetLogger(MemcachedProxy.MemcachedLogger).Trace("Create Memcache : {0}", this.Name);
            LogManager.GetLogger(MemcachedProxy.MemcachedLogger).Trace("\treadServers : {0}", readServers.ToText());
            LogManager.GetLogger(MemcachedProxy.MemcachedLogger).Trace("\twriteServers : {0}", writeServers.ToText());
        }

        #endregion

        #region AbstractCache 成员

        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public override object GetItem(string key)
        {
            var cacheKey = FormatKey(key);
            var value = this.readProxy.Get(cacheKey);
            return value;
        }

        /// <summary>
        /// 保存数据
        /// </summary>
        /// <param name="key"></param>
        /// <param name="data"></param>
        /// <param name="expiryTime"></param>
        public override void SetItem(string key, object data, TimeSpan expiryTime)
        {
            var cacheKey = FormatKey(key);
            this.writeProxy.Set(cacheKey, data, this.ExpiryTime);
        }

        /// <summary>
        /// 移除数据
        /// </summary>
        /// <param name="key"></param>
        public override void RemoveItem(string key)
        {
            var cacheKey = FormatKey(key);
            this.writeProxy.Delete(cacheKey);
        }

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
            return this.readProxy.Stats();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        public virtual long? Increment(string key, int amount)
        {
            var cacheKey = FormatKey(key);
            return (long)writeProxy.Increment(cacheKey, (uint)amount);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="keys"></param>
        /// <returns></returns>
        public object[] Get(string[] keys)
        {
            if (keys != null && keys.Length > 0)
            {
                for(var i=0;i<keys.Length;i++)
                    keys[i] = FormatKey(keys[i]);
            }
            return readProxy.Get(keys);
        }

        #endregion

        private static string FormatKey(string key)
        {
            return key.Replace(" ", "~");
        }
    }
}
