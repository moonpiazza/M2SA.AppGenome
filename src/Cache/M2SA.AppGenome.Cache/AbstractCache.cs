using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;
using M2SA.AppGenome.Configuration;

namespace M2SA.AppGenome.Cache
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class AbstractCache : ResolveObjectBase, ICache
    {
        ICacheNotify cacheNotify;

        /// <summary>
        /// 缓存监听接口的实现
        /// </summary>
        public ICacheNotify Notify
        {
            get { return this.cacheNotify; }
            set
            {
                this.cacheNotify = value;
                if (null != value)
                {
                    this.cacheNotify.SetCache(this);
                }
            }
        }

        /// <summary>
        /// 大于指定的字节数，则启用压缩保存
        /// </summary>
        public int CompressionThreshold
        {
            get;
            set;
        }

        #region ICache 成员

        /// <summary>
        /// 缓存项操作接口
        /// </summary>
        public ILoadDataHandler LoadDataHandler
        {
            get;
            set;
        }

        /// <summary>
        /// 缓存名称
        /// </summary>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// 缓存有效期
        /// </summary>
        public TimeSpan ExpiryTime
        {
            get;
            set;
        }

        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public object Get(string key)
        {
            if (null != this.Notify) this.Notify.OnBeforeGet(key);
            var value = this.GetItem(key);
            if (null != this.Notify) this.Notify.OnAfterGet(key, ref value);

            return value;
        }

        /// <summary>
        /// 获取数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public T Get<T>(object key)
        {
            ArgumentAssertion.IsNotNull(key, "oKey");
            var cacheKey = key.ToString();
            var val = this.Get(cacheKey);
            if (val == null)
                return default(T);
            else
                return (T)val;
        }

        /// <summary>
        /// 保存数据
        /// </summary>
        /// <param name="key"></param>
        /// <param name="data"></param>
        public void Set(string key, object data)
        {
            if (null == data)
            {
                this.Remove(key);
            }
            else
            {
                if (null != this.Notify) this.Notify.OnBeforeSet(key, data);
                this.SetItem(key, data, this.ExpiryTime);
                if (null != this.Notify) this.Notify.OnAfterSet(key, data);
            }
        }

        /// <summary>
        /// 保存数据
        /// </summary>
        /// <param name="key"></param>
        /// <param name="data"></param>
        public void Set(object key, object data)
        {
            ArgumentAssertion.IsNotNull(key, "key");
            var cacheKey = key.ToString();
            this.Set(cacheKey, data);
        }

        /// <summary>
        /// 移除数据
        /// </summary>
        /// <param name="key"></param>
        public void Remove(string key)
        {
            if (null != this.Notify) this.Notify.OnBeforeRemove(key);
            this.RemoveItem(key);
            if (null != this.Notify) this.Notify.OnAfterRemove(key);
        }

        /// <summary>
        /// 清除缓存
        /// </summary>
        public abstract void Clear();

        /// <summary>
        /// 获取缓存状态信息
        /// </summary>
        /// <returns></returns>
        public abstract object GetState();

        #endregion

        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public abstract object GetItem(string key);

        /// <summary>
        /// 保存数据
        /// </summary>
        /// <param name="key"></param>
        /// <param name="data"></param>
        /// <param name="expiryTime"></param>
        public abstract void SetItem(string key, object data, TimeSpan expiryTime);

        /// <summary>
        /// 移除数据
        /// </summary>
        /// <param name="key"></param>
        public abstract void RemoveItem(string key);
    }
}
