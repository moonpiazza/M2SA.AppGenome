using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace M2SA.AppGenome.Data
{
    /// <summary>
    /// 
    /// </summary>
    public static class RepositoryManager
    {
        /// <summary>
        /// 获取IRepository的默认实例
        /// </summary>
        /// <returns></returns>
        public static TRepository GetRepository<TRepository>()
        {
            return ObjectIOCFactory.GetSingleton<IRepositoryFactory>().GetRepository<TRepository>();
        }


        /// <summary>
        /// 获取IRepository的实例
        /// </summary>
        /// <returns></returns>
        public static TRepository GetRepository<TRepository>(string categoryName)
        {
            return ObjectIOCFactory.GetSingleton<IRepositoryFactory>("RepositoryFactory." + categoryName).GetRepository<TRepository>();
        }
    }
}
