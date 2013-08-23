using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using M2SA.AppGenome.Configuration;

namespace M2SA.AppGenome.Cache
{
    /// <summary>
    /// 
    /// </summary>
    public class CacheFactory : ResolveFactoryBase<ICache>, ICacheFactory
    {
        /// <summary>
        /// 
        /// </summary>
        protected override string ModuleKey
        {
            get
            {
                return AppConfig.CachedKey;
            }
        }

        private CacheFactory()
        {
            AppInstance.RegisterTypeAliasByModule<MemCached.MemCache>(AppConfig.CachedKey);
            AppInstance.RegisterTypeAliasByModule<AppDomainCache>(AppConfig.CachedKey);
            AppInstance.RegisterTypeAliasByModule<ActiveCacheNotify>(AppConfig.CachedKey);
        }


        /// <summary>
        /// 加载指定的配置信息
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        protected override IConfigNode LoadConfigInfo(string name)
        {
            var nodePath = string.Format("/cache[@name='{0}']", name);
            var configNode = AppInstance.GetConfigNode(this.ModuleKey, nodePath);
            return configNode;
        }

        #region ICacheFactory 成员

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ICache GetCache()
        {
            return this.GetInstance();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="categoryName"></param>
        /// <returns></returns>
        public ICache GetCache(string categoryName)
        {
            return this.GetInstance(categoryName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="categoryName"></param>
        /// <returns></returns>
        public bool ExistsCache(string categoryName)
        {
            return this.Exists(categoryName);
        }

        #endregion
    }
}
