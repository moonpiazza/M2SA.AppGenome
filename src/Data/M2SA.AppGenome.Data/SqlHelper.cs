using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Configuration;
using System.Reflection;
using System.Data;
using System.Data.Common;
using M2SA.AppGenome.Data.SqlMap;

namespace M2SA.AppGenome.Data
{
    /// <summary>
    /// 数据访问适配器
    /// </summary>
    public static class SqlHelper
    {
        static SqlHelper()
        {
            AppInstance.RegisterTypeAlias<MySql.MySqlProvider>(typeof(MySql.MySqlProvider).Name);
            AppInstance.RegisterTypeAlias<SqlServer.SqlServerProvider>(typeof(SqlServer.SqlServerProvider).Name);                  
        }

        /// <summary>
        /// 
        /// </summary>
        public readonly static string SqlLogger = "Sql";

        #region ExecuteNonQuery Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sqlName"></param>
        /// <param name="parameterValues"></param>
        /// <returns></returns>
        public static int ExecuteNonQuery(string sqlName, IDictionary<string, object> parameterValues)
        {
            var rowCount = ExecuteNonQuery(sqlName, parameterValues, null);
            return rowCount;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sqlName"></param>
        /// <param name="parameterValues"></param>
        /// <param name="partitionValues"></param>
        /// <returns></returns>
        public static int ExecuteNonQuery(string sqlName, IDictionary<string, object> parameterValues, string partitionValues)
        {
            var sqlWrap = SqlMapping.GetSqlWrap(sqlName);
            return ExecuteNonQuery(sqlWrap, parameterValues, partitionValues);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameterValues"></param>
        /// <returns></returns>
        public static int ExecuteNonQuery(SqlWrap sql, IDictionary<string, object> parameterValues)
        {
            return ExecuteNonQuery(sql, parameterValues, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameterValues"></param>
        /// <param name="partitionValues"></param>
        /// <returns></returns>
        public static int ExecuteNonQuery(SqlWrap sql, IDictionary<string, object> parameterValues, string partitionValues)
        {
            ArgumentAssertion.IsNotNull(sql, "sql");

            var dbProvider = GetDatabaseProvider(sql, partitionValues);
            var rowCount = dbProvider.ExecuteNonQuery(sql.SQLText, parameterValues, sql.CommandType, sql.CommandTimeout);
            return rowCount;
        }

        #endregion

        #region ExcuteTableForPage Methods


        #endregion

        #region ExecuteDataSet Methods

        /// <summary>
        /// 通过指定SQL名称获取SQL语句，执行后返回查询数据
        /// </summary>
        /// <param name="sqlName"></param>
        /// <param name="parameterValues"></param>
        /// <returns></returns>
        public static DataSet ExecuteDataSet(string sqlName, IDictionary<string, object> parameterValues)
        {
            return ExecuteDataSet(sqlName, parameterValues, null);
        }

        /// <summary>
        /// 通过指定SQL名称获取SQL语句，执行后返回查询数据
        /// </summary>
        /// <param name="sqlName"></param>
        /// <param name="parameterValues"></param>
        /// <param name="partitionValues">分区字段值列表</param>
        /// <returns></returns>
        public static DataSet ExecuteDataSet(string sqlName, IDictionary<string, object> parameterValues, string partitionValues)
        {
            var sqlWrap = SqlMapping.GetSqlWrap(sqlName);
            return ExecuteDataSet(sqlWrap, parameterValues, partitionValues);
        }

        /// <summary>
        /// 通过指定SqlWrap，执行后返回查询数据
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameterValues"></param>
        /// <returns></returns>
        public static DataSet ExecuteDataSet(SqlWrap sql, IDictionary<string, object> parameterValues)
        {
            return ExecuteDataSet(sql, parameterValues, null);
        }

        /// <summary>
        /// 通过指定SqlWrap，执行后返回查询数据
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameterValues"></param>
        /// <param name="partitionValues">分区字段值列表</param>
        /// <returns></returns>
        public static DataSet ExecuteDataSet(SqlWrap sql, IDictionary<string, object> parameterValues, string partitionValues)
        {
            ArgumentAssertion.IsNotNull(sql, "sql");

            var dbProvider = GetDatabaseProvider(sql, partitionValues);
            var dataset = dbProvider.ExecuteDataSet(sql.SQLText, parameterValues, sql.CommandType, sql.CommandTimeout);
            return dataset;
        }

        #endregion

        #region ExecuteScalar Methods


        /// <summary>
        /// 通过指定SQL名称获取SQL语句，执行后返回单值
        /// </summary>
        /// <param name="sqlName"></param>
        /// <param name="parameterValues"></param>
        /// <returns></returns>
        public static object ExecuteScalar(string sqlName, IDictionary<string, object> parameterValues)
        {
            return ExecuteScalar(sqlName, parameterValues, null);
        }

        /// <summary>
        /// 通过指定SQL名称获取SQL语句，执行后返回单值
        /// </summary>
        /// <param name="sqlName"></param>
        /// <param name="parameterValues"></param>
        /// <param name="partitionValues">分区字段值列表</param>
        /// <returns></returns>
        public static object ExecuteScalar(string sqlName, IDictionary<string, object> parameterValues, string partitionValues)
        {
            var sqlWrap = SqlMapping.GetSqlWrap(sqlName);
            return ExecuteScalar(sqlWrap, parameterValues, partitionValues);
        }

        /// <summary>
        /// 通过指定SqlWrap，执行后返回单值
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameterValues"></param>
        /// <returns></returns>
        public static object ExecuteScalar(SqlWrap sql, IDictionary<string, object> parameterValues)
        {
            return ExecuteScalar(sql, parameterValues, null);
        }

        /// <summary>
        /// 通过指定SqlWrap，执行后返回单值
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameterValues"></param>
        /// <param name="partitionValues">分区字段值列表</param>
        /// <returns></returns>
        public static object ExecuteScalar(SqlWrap sql, IDictionary<string, object> parameterValues, string partitionValues)
        {
            ArgumentAssertion.IsNotNull(sql, "sql");

            var dbProvider = GetDatabaseProvider(sql, partitionValues);
            var result = dbProvider.ExecuteScalar(sql.SQLText, parameterValues, sql.CommandType, sql.CommandTimeout);
            return result;
        }

        #endregion        

        #region ExecuteReader Methods


        /// <summary>
        /// 通过指定SQL名称执行SQL语句，获得DbDataReader后，使用委托Action[DbDataReader]处理数据流
        /// </summary>
        /// <param name="sqlName"></param>
        /// <param name="parameterValues"></param>
        /// <param name="action"></param>
        public static void ExecuteReader(string sqlName, IDictionary<string, object> parameterValues, Action<DbDataReader> action)
        {
            ExecuteReader(sqlName, parameterValues, null, action);
        }

        /// <summary>
        /// 通过指定SQL名称执行SQL语句，获得DbDataReader后，使用委托Action[DbDataReader]处理数据流
        /// </summary>
        /// <param name="sqlName"></param>
        /// <param name="parameterValues"></param>
        /// <param name="partitionValues"></param>
        /// <param name="action"></param>
        public static void ExecuteReader(string sqlName, IDictionary<string, object> parameterValues, string partitionValues, Action<DbDataReader> action)
        {
            var sqlWrap = SqlMapping.GetSqlWrap(sqlName);
            ExecuteReader(sqlWrap, parameterValues, partitionValues, action);
        }

        /// <summary>
        /// 通过指定SQL名称执行SQL语句，获得DbDataReader后，使用委托Action[DbDataReader]处理数据流
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameterValues"></param>
        /// <param name="action"></param>
        public static void ExecuteReader(SqlWrap sql, IDictionary<string, object> parameterValues, Action<DbDataReader> action)
        {
            ExecuteReader(sql, parameterValues, null, action);
        }

        /// <summary>
        /// 通过指定SQL名称执行SQL语句，获得DbDataReader后，使用委托Action[DbDataReader]处理数据流
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameterValues"></param>
        /// <param name="partitionValues"></param>
        /// <param name="action"></param>
        public static void ExecuteReader(SqlWrap sql, IDictionary<string, object> parameterValues, string partitionValues, Action<DbDataReader> action)
        {
            ArgumentAssertion.IsNotNull(sql, "sql");

            if (null == action)
                throw new ArgumentNullException("action");

            var dbProvider = GetDatabaseProvider(sql, partitionValues);
            using (var dbReader = dbProvider.ExecuteReader(sql.SQLText, parameterValues, sql.CommandType, sql.CommandTimeout))
            {
                action(dbReader);
            }
        }

        #endregion        

        #region Helper Methods

        static IDatabaseProvider GetDatabaseProvider(SqlWrap sqlWrap, string partitionValues)
        {
            var dbConfig = SqlMapping.GetDatabase(sqlWrap.DBName);
            if ((dbConfig.DBType & sqlWrap.SupportDBType) == 0)
            {
                throw new ArgumentOutOfRangeException(string.Format("SqlWrap[{0}] 不支持类型为{1}的数据库[{2}]", sqlWrap.SQLName, dbConfig.DBType, sqlWrap.DBName));
            }

            var dbProvider = DatabaseProviderFactory.GetDatabaseProvider(dbConfig);
            return dbProvider;
        }

        static DatabaseType GetDatabaseTypeFromConfig(string configName)
        {
            var dbType = SqlMapping.DefaultDbType;
            string appSettingKey = string.Format("{0}.DatabaseType", configName);
            var dbTypeStr = ConfigurationManager.AppSettings[appSettingKey];

            if (string.IsNullOrEmpty(dbTypeStr) == false)
            {
                dbType = (DatabaseType)Enum.Parse(typeof(DatabaseType), dbTypeStr);
            }

            return dbType;
        }

        #endregion
    }
}
