using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace M2SA.AppGenome.Data.SqlMap
{
    /// <summary>
    /// 
    /// </summary>
    public static class SqlMapping
    {
        private static readonly object sync = new object();

        /// <summary>
        /// 默认的数据库类型
        /// </summary>
        public static readonly DatabaseType DefaultDbType = DatabaseType.MySql;

        /// <summary>
        /// 存储数据库的映射字典（数据库级别）
        /// </summary>
        private static IDictionary<string, DatabaseConfig> DBMap { get; set; }

        /// <summary>
        /// 存储数据模块的配置节点（通常是表级别）
        /// </summary>
        private static IDictionary<string, string> ModuleMap { get; set; }

        /// <summary>
        /// 存储Sql语句的映射字典（具体的Sql语句级别）
        /// </summary>
        private static IDictionary<string, SqlWrap> SqlMap { get; set; }

        static SqlMapping()
        {
            Initialize();
        }

        /// <summary>
        /// 
        /// </summary>
        public static void Initialize()
        {
            try
            {
                if (null == DBMap)
                {
                    lock (sync)
                    {
                        if (DBMap == null)
                        {
                            DBMap = new Dictionary<string, DatabaseConfig>(8);
                            ModuleMap = new Dictionary<string, string>(32);
                            SqlMap = new Dictionary<string, SqlWrap>(128);
                            SqlMappingLoader.LoadSqlMapping();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logging.LogManager.GetLogger(SqlHelper.SqlLogger).Error(ex, "LoadSqlMap errors.");
                throw;
            }
        }

        #region Append Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="databases"></param>
        public static void AppendDatabases(IList<DatabaseConfig> databases)
        {
            if (null == databases || databases.Count == 0)
                return;
            foreach (var item in databases)
            {
                AppendDatabaseConfig(item);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="modules"></param>
        public static void AppendModules(IDictionary<string, string> modules)
        {
            if (null == modules || modules.Count == 0)
                return;
            foreach (var item in modules)
            {
                AppendModule(item.Key, item.Value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sqlWraps"></param>
        public static void AppendSqlWraps(IList<SqlWrap> sqlWraps)
        {
            if (null == sqlWraps || sqlWraps.Count == 0)
                return;
            foreach (var item in sqlWraps)
            {
                AppendSqlWrap(item);
            }
        }

        private static void AppendDatabaseConfig(DatabaseConfig dbConfig)
        {
            var dbKey = dbConfig.ConfigName.ToLower();
            if (DBMap.ContainsKey(dbKey))
                Logging.LogManager.GetLogger(SqlHelper.SqlLogger).Warn("[DBMap]{0} are covered.", dbConfig.ConfigName);
            DBMap[dbKey] = dbConfig;
        }

        private static void AppendModule(string moduleName, string dbName)
        {
            var moduleKey = moduleName.ToLower();
            if (ModuleMap.ContainsKey(moduleKey))
                Logging.LogManager.GetLogger(SqlHelper.SqlLogger).Warn("[ModuleMap]{0} are covered. {1} => {2}", moduleKey, ModuleMap[moduleKey], dbName);
            ModuleMap[moduleKey] = dbName;
        }

        private static void AppendSqlWrap(SqlWrap sqlWrap)
        {
            var sqlKey = sqlWrap.SqlName.ToLower();
            if (SqlMap.ContainsKey(sqlKey))
                Logging.LogManager.GetLogger(SqlHelper.SqlLogger).Warn("[SqlMap]{0} are covered.", sqlWrap.SqlName);
            SqlMap[sqlKey] = sqlWrap;
        }
        
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbName"></param>
        /// <returns></returns>
        public static bool ExistDatabase(string dbName)
        {
            ArgumentAssertion.IsNotNull(dbName, "dbName");

            var dbKey = dbName.ToLower();
            return DBMap.ContainsKey(dbKey);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbName"></param>
        /// <returns></returns>
        public static DatabaseConfig GetDatabase(string dbName)
        {
            ArgumentAssertion.IsNotNull(dbName, "dbName");

            var dbKey = dbName.ToLower();
            if (DBMap.ContainsKey(dbKey) == false)
                throw new ArgumentOutOfRangeException("dbName", dbName, string.Format("没有定义数据库 - {0}", dbName));

            return DBMap[dbKey];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="moduleName"></param>
        /// <returns></returns>
        public static string GetDbNameByModule(string moduleName)
        {
            ArgumentAssertion.IsNotNull(moduleName, "moduleName");

            var moduleKey = moduleName.ToLower();
            if (ModuleMap.ContainsKey(moduleKey) == false)
                throw new ArgumentOutOfRangeException("moduleName", moduleName, string.Format("没有定义模块 - {0}", moduleName));

            return ModuleMap[moduleKey];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sqlName"></param>
        /// <returns></returns>
        public static SqlWrap GetSqlWrap(string sqlName)
        {
            Initialize();

            ArgumentAssertion.IsNotNull(sqlName, "sqlName");
            var sqlKey = sqlName.ToLower();
            if (SqlMap.ContainsKey(sqlKey) == false)
                throw new ArgumentOutOfRangeException("sqlName", sqlName, string.Format("没有定义SqlWrap - {0}", sqlName));
            return SqlMap[sqlKey];
        }

    }
}
