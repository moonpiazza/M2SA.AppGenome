using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace M2SA.AppGenome.Cache
{
    /// <summary>
    /// 获取ICache的默认实例
    /// </summary>
    public static class CacheManager
    {
        /// <summary>
        /// 获取ICache的默认实例
        /// </summary>
        /// <returns></returns>
        public static ICache GetCache()
        {
            return ObjectIOCFactory.GetSingleton<ICacheFactory>().GetCache();
        }

        /// <summary>
        /// 获取ICache的指定类别的实例
        /// </summary>
        /// <param name="categoryName"></param>
        /// <returns></returns>
        public static ICache GetCache(string categoryName)
        {
            return ObjectIOCFactory.GetSingleton<ICacheFactory>().GetCache(categoryName);
        }

        /// <summary>
        /// 是否存在指定类别的ICache
        /// </summary>
        /// <param name="categoryName"></param>
        /// <returns></returns>
        public static bool ExistsCache(string categoryName)
        {
            return ObjectIOCFactory.GetSingleton<ICacheFactory>().ExistsCache(categoryName);
        }
    }
}
