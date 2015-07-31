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
        private static readonly object Sync = new object();

        /// <summary>
        /// 
        /// </summary>
        public static readonly string ModuleKeySeparator = ".";

        /// <summary>
        /// 
        /// </summary>
        public static readonly string SqKeySeparator = ":";

        /// <summary>
        /// 
        /// </summary>
        public static readonly string SqlKeyForDbSeparator = "|";

        /// <summary>
        /// 默认的数据库类型
        /// </summary>
        public static readonly DatabaseType DefaultDbType = DatabaseType.MySql;

        /// <summary>
        /// 
        /// </summary>
        private static bool IsLoadedSqlMap = false;

        /// <summary>
        /// 存储数据库的映射字典（数据库级别）
        /// </summary>
        private static IDictionary<string, DatabaseConfig> DbMap { get; set; }

        /// <summary>
        /// 
        /// </summary>
        private static IDictionary<string, DatabaseConfig> ModuleDbMap { get; set; }

        /// <summary>
        /// 存储模块/Sql语句的配置节点
        /// </summary>
        private static IDictionary<string, SqlModule> SqlMap { get; set; }

        static SqlMapping()
        {
            //Initialize();
        }

        /// <summary>
        /// 
        /// </summary>
        public static void Initialize()
        {
            try
            {
                if (false == IsLoadedSqlMap)
                {
                    lock (Sync)
                    {
                        if (false == IsLoadedSqlMap)
                        {
                            DbMap = new Dictionary<string, DatabaseConfig>(8);
                            SqlMap = new Dictionary<string, SqlModule>(32);
                            SqlMappingLoader.LoadSqlMapping();
                            IsLoadedSqlMap = true;
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

            if (null == DbMap) Initialize();
            foreach (var item in databases)
            {
                AppendDatabaseConfig(item);
            }
        }

        private static void AppendDatabaseConfig(DatabaseConfig dbConfig)
        {
            var dbKey = dbConfig.ConfigName.ToLower();
            if (DbMap.ContainsKey(dbKey))
                Logging.LogManager.GetLogger(SqlHelper.SqlLogger).Warn("[DbMap]{0} are covered.", dbConfig.ConfigName);
            DbMap[dbKey] = dbConfig;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sqlModules"></param>
        public static void AppendSqlModules(IDictionary<string, SqlModule> sqlModules)
        {
            if (null == sqlModules || sqlModules.Count == 0)
                return;

            if (null == DbMap) Initialize();
            foreach (var item in sqlModules)
            {
                AppendSqlModule(item.Key, item.Value);
            }
        }

        private static void AppendSqlModule(string moduleName, SqlModule sqlModule)
        {
            var moduleKey = moduleName.ToLower();

            if (SqlMap.ContainsKey(moduleKey))
            {
                var module = SqlMap[moduleKey];
                foreach (var item in sqlModule.SqlMap)
                {
                    module.AddSqlWrap(item.Value);
                }
            }
            else
            {
                SqlMap.Add(moduleKey, sqlModule);
            }
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
            if (null == DbMap) Initialize();

            var dbKey = dbName.ToLower();
            return DbMap.ContainsKey(dbKey);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbName"></param>
        /// <returns></returns>
        public static DatabaseConfig GetDatabase(string dbName)
        {
            Initialize();

            ArgumentAssertion.IsNotNull(dbName, "dbName");

            var dbKey = dbName.ToLower();
            if (DbMap.ContainsKey(dbKey) == false)
                throw new ArgumentOutOfRangeException("dbName", dbName, string.Format("没有定义数据库 - {0}", dbName));

            return DbMap[dbKey];
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

            SqlWrap sqlWrap = null;

            if (sqlName.Contains(SqlMapping.SqKeySeparator))
            {
                var keyParts = sqlName.ToLower().Split(SqlMapping.SqKeySeparator[0]);
                var sqlMap = FindSqlModule(keyParts[0]);
                if (null != sqlMap)
                {
                    sqlWrap = FindSqlWrap(string.Join(SqlMapping.ModuleKeySeparator, keyParts, 1, keyParts.Length-1), sqlMap);
                }
            }
            else if (sqlName.Contains(SqlMapping.ModuleKeySeparator)) //兼容老版本直接调用SqlHelper的逻辑
            {
                var keyParts = sqlName.ToLower().Split(SqlMapping.ModuleKeySeparator[0]);
                var sqlMap = FindSqlModule(string.Join(SqlMapping.ModuleKeySeparator, keyParts, 0, keyParts.Length-1));
                if (null != sqlMap)
                {
                    sqlWrap = FindSqlWrap(keyParts[keyParts.Length-1], sqlMap);
                }
            }
            

            if (null == sqlWrap)
                throw new ArgumentOutOfRangeException("sqlName", sqlName, string.Format("没有定义SqlWrap - {0}", sqlName));            

            return sqlWrap;
        }

        private static SqlModule FindSqlModule(string moduleKey)
        {
            if (SqlMap.ContainsKey(moduleKey))
                return SqlMap[moduleKey];

            //A.B.Module, A.Module, Module
            var keyParts = moduleKey.Split(SqlMapping.ModuleKeySeparator[0]);
            if (keyParts.Length < 2) return null;
            
            var moduleName = keyParts[keyParts.Length - 1];
            for (var i = keyParts.Length-2; i >= 0; i--)
            {
                if (i>0)
                    moduleKey = string.Concat(string.Join(SqlMapping.ModuleKeySeparator, keyParts, 0, i), SqlMapping.ModuleKeySeparator, moduleName);
                else
                    moduleKey = moduleName;
                if (SqlMap.ContainsKey(moduleKey))
                {
                    return SqlMap[moduleKey];
                }
            }
            return null;
        }

        private static SqlWrap FindSqlWrap(string sqlName, SqlModule sqlModule)
        {
            var database = GetDatabase(sqlModule.DbName);
            var sqlKey = string.Concat(sqlName, SqlMapping.SqlKeyForDbSeparator, database.DBType);
            if (sqlModule.SqlMap.ContainsKey(sqlKey))
                return sqlModule.SqlMap[sqlKey];
            else if (sqlModule.SqlMap.ContainsKey(sqlName))
                return sqlModule.SqlMap[sqlName];
            else
                return null;
        }
    }
}
