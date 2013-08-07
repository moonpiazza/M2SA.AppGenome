using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace M2SA.AppGenome.Cache
{
    /// <summary>
    /// 
    /// </summary>
    public interface ICacheFactory
    {
        /// <summary>
        /// 获取ICache的默认实例
        /// </summary>
        /// <returns></returns>
        ICache GetCache();

        /// <summary>
        /// 获取ICache的指定类别的实例
        /// </summary>
        /// <param name="categoryName"></param>
        /// <returns></returns>
        ICache GetCache(string categoryName);

        /// <summary>
        /// 是否存在指定类别的ICache
        /// </summary>
        /// <param name="categoryName"></param>
        /// <returns></returns>
        bool ExistsCache(string categoryName);
    }
}
