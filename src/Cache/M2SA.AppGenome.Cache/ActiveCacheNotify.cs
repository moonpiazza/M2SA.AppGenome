using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;
using System.Threading;

namespace M2SA.AppGenome.Cache
{
    /// <summary>
    /// 保持缓存定时更新的ICacheNotify的实现
    /// </summary>
    public class ActiveCacheNotify : ICacheNotify
    {
        ICache cache = null;
        string taskGroupName = null;

        /// <summary>
        /// 
        /// </summary>
        public bool KeepActive
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public TimeSpan RefreshInterval
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public ActiveCacheNotify()
        {
        }

        bool Enable()
        {
            return this.KeepActive && this.RefreshInterval > TimeSpan.Zero;
        }

        #region ICacheNotify 成员

        /// <summary>
        /// 
        /// </summary>
        public bool NeedItemHandler
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="instance"></param>
        public void SetCache(ICache instance)
        {
            this.cache = instance;
            if (this.RefreshInterval <= TimeSpan.Zero)
            {
                var interval = this.cache.ExpiryTime.TotalSeconds * 0.9;
                if (interval < int.MaxValue && interval > int.MinValue)
                {
                    var intervalSpan = TimeSpan.FromSeconds(interval);
                    if (this.RefreshInterval <= TimeSpan.Zero || this.RefreshInterval > intervalSpan)
                    {
                        this.RefreshInterval = intervalSpan;
                    }
                }
            }

            if (Enable())
            {
                this.taskGroupName = string.Format("{0}.{1}", AppConfig.CachedKey, this.cache.Name);
                AppInstance.GetTaskProcessor().RegisterAction<string>(this.taskGroupName, this.ResetCacheItem, this.RefreshInterval, true);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        public void OnBeforeGet(string key)
        { /* empty action */
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void OnAfterGet(string key, ref object value)
        { /* empty action */
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void OnBeforeSet(string key, object value)
        { /* empty action */
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void OnAfterSet(string key, object value)
        {
            if (Enable())
            {
                AppInstance.GetTaskProcessor().Process<string>(this.taskGroupName, key);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        public void OnBeforeRemove(string key)
        { /* empty action */
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        public void OnAfterRemove(string key)
        { /* empty action */
        }

        #endregion

        void ResetCacheItem(string key)
        {
            var data = this.cache.LoadDataHandler.LoadData(key);
            cache.Set(key, data);
        }
    }
}
