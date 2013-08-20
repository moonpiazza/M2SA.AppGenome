using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace M2SA.AppGenome.Cache
{
    /// <summary>
    /// 
    /// </summary>
    public static class CacheExtend
    {
        /// <summary>
        /// 从缓存获取数据，如缓存中没有指定数据，则通过loadEntity(key)获取数据，并将数据缓存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cache"></param>
        /// <param name="key"></param>
        /// <param name="loadEntity"></param>
        /// <returns></returns>
        public static T LoadEntityAndCache<T>(this ICache cache, string key, Func<T> loadEntity)
        {
            var entity = cache.Get<T>(key);
            if (null == entity)
            {
                entity = loadEntity();
                if (null != entity)
                    cache.Set(key, entity);
            }
            return entity;
        }
    }
}
