using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using M2SA.AppGenome.Reflection;

namespace M2SA.AppGenome.Data
{
    /// <summary>
    /// 
    /// </summary>
    public static class DatabaseProviderFactory
    {
        static readonly object syncRoot = new object();
        static readonly IDictionary<string, Type> providerTypes = new Dictionary<string, Type>(10);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2208:InstantiateArgumentExceptionsCorrectly")]
        public static IDatabaseProvider GetDatabaseProvider(DatabaseConfig config)
        {
            ArgumentAssertion.IsNotNull(config, "config");
            var providerName = config.ProviderName;
            if (providerTypes.ContainsKey(providerName) == false)
            {
                lock (syncRoot)
                {
                    if (providerTypes.ContainsKey(providerName) == false)
                    {
                        var providerType = TypeExtension.GetMapType(providerName);
                        if (null == providerType)
                        {
                            throw new ArgumentOutOfRangeException("providerName", providerName
                                , string.Format("没有定义数据库{0}的驱动:{1}", config.ConfigName, providerName));
                        }
                        else
                        {
                            providerTypes[providerName] = providerType;
                        }
                    }
                }
            }

            var dbPrproviderType = providerTypes[providerName];
            var dbProvider = (IDatabaseProvider)Activator.CreateInstance(dbPrproviderType);
            dbProvider.ConnectionString = config.ConnectionString;
            return dbProvider;
        }
    }
}
