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
using M2SA.AppGenome.Logging;

namespace M2SA.AppGenome.Data
{
    /// <summary>
    /// 数据访问适配器
    /// </summary>
    public static class SqlHelper
    {
        static SqlHelper()
        {
            AppInstance.RegisterTypeAlias<MySql.MySqlProvider>();
            AppInstance.RegisterTypeAlias<SqlServer.SqlServerProvider>();                  
        }

        /// <summary>
        /// 
        /// </summary>
        public readonly static string SqlLogger = "Sql";

        #region ExecuteIdentity<T> Methods


        /// <summary>
        /// 通过指定SQL名称获取SQL语句，执行后返回自增值
        /// </summary>
        /// <param name="sqlName"></param>
        /// <param name="parameterValues"></param>
        /// <returns></returns>
        public static T ExecuteIdentity<T>(string sqlName, IDictionary<string, object> parameterValues)
        {
            return ExecuteIdentity<T>(sqlName, parameterValues, null);
        }

        /// <summary>
        /// 通过指定SQL名称获取SQL语句，执行后返回自增值
        /// </summary>
        /// <param name="sqlName"></param>
        /// <param name="parameterValues"></param>
        /// <param name="partitionValues">分区字段值列表</param>
        /// <returns></returns>
        public static T ExecuteIdentity<T>(string sqlName, IDictionary<string, object> parameterValues, string partitionValues)
        {
            var sqlWrap = SqlMapping.GetSqlWrap(sqlName);
            return ExecuteIdentity<T>(sqlWrap, parameterValues, partitionValues);
        }

        /// <summary>
        /// 通过指定SqlWrap，执行后返回自增值
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameterValues"></param>
        /// <returns></returns>
        public static T ExecuteIdentity<T>(SqlWrap sql, IDictionary<string, object> parameterValues)
        {
            return ExecuteIdentity<T>(sql, parameterValues, null);
        }

        /// <summary>
        /// 通过指定SqlWrap，执行后返回自增值
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameterValues"></param>
        /// <param name="partitionValues">分区字段值列表</param>
        /// <returns></returns>
        public static T ExecuteIdentity<T>(SqlWrap sql, IDictionary<string, object> parameterValues, string partitionValues)
        {
            ArgumentAssertion.IsNotNull(sql, "sql");

            var dbProvider = GetDatabaseProvider(sql, partitionValues);

            try
            {
                var identity = dbProvider.ExecuteIdentity<T>(sql, parameterValues);
                return identity;
            }
            catch (Exception ex)
            {
                throw BuildSqlWrapException(ex, sql, parameterValues);
            }
        }

        #endregion

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

            try
            {
                var rowCount = dbProvider.ExecuteNonQuery(sql, parameterValues);
                return rowCount;
            }
            catch (Exception ex)
            {
                throw BuildSqlWrapException(ex, sql, parameterValues);
            }
        }

        #endregion

        #region ExcuteTableForPage Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sqlName"></param>
        /// <param name="parameterValues"></param>
        /// <param name="pagination"></param>
        /// <returns></returns>
        public static DataTable ExecutePaginationTable(string sqlName, IDictionary<string, object> parameterValues, Pagination pagination)
        {
            var sqlWrap = SqlMapping.GetSqlWrap(sqlName);
            return ExecutePaginationTable(sqlWrap, parameterValues, pagination, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sqlName"></param>
        /// <param name="parameterValues"></param>
        /// <param name="pagination"></param>
        /// <param name="partitionValues"></param>
        /// <returns></returns>
        public static DataTable ExecutePaginationTable(string sqlName, IDictionary<string, object> parameterValues, Pagination pagination, string partitionValues)
        {
            var sqlWrap = SqlMapping.GetSqlWrap(sqlName);
            return ExecutePaginationTable(sqlWrap, parameterValues, pagination, partitionValues);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameterValues"></param>
        /// <param name="pagination"></param>
        /// <returns></returns>
        public static DataTable ExecutePaginationTable(SqlWrap sql, IDictionary<string, object> parameterValues, Pagination pagination)
        {
            return ExecutePaginationTable(sql, parameterValues, pagination, null);
        }

        /// <summary>
        /// 通过指定SqlWrap，执行后返回查询数据
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameterValues"></param>
        /// <param name="pagination"></param>
        /// <param name="partitionValues">分区字段值列表</param>
        /// <returns></returns>
        public static DataTable ExecutePaginationTable(SqlWrap sql, IDictionary<string, object> parameterValues, Pagination pagination, string partitionValues)
        {
            ArgumentAssertion.IsNotNull(sql, "sql");

            var dbProvider = GetDatabaseProvider(sql, partitionValues);
            try
            {
                var dataset = dbProvider.ExecutePaginationTable(sql, parameterValues, pagination);
                return dataset;
            }
            catch (Exception ex)
            {
                throw BuildSqlWrapException(ex, sql, parameterValues);
            }
        }

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

            try
            {
                var dataset = dbProvider.ExecuteDataSet(sql, parameterValues);
                return dataset;
            }
            catch (Exception ex)
            {
                throw BuildSqlWrapException(ex, sql, parameterValues);
            }
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

            try
            {
                var result = dbProvider.ExecuteScalar(sql, parameterValues);
                return result;
            }
            catch (Exception ex)
            {
                throw BuildSqlWrapException(ex, sql, parameterValues);
            }
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

            try
            {
                using (var dbReader = dbProvider.ExecuteReader(sql, parameterValues))
                {
                    action(dbReader);
                }
            }
            catch (Exception ex)
            {
                throw BuildSqlWrapException(ex, sql, parameterValues);
            }
        }

        #endregion        

        #region Helper Methods

        private static SqlWrapException BuildSqlWrapException(Exception ex, SqlWrap sqlWrap, IDictionary<string, object> parameterValues)
        {
            if (ex is SqlWrapException) 
                return (SqlWrapException) ex;
            else
                return new SqlWrapException(ex, sqlWrap.FullName, parameterValues);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "partitionValues")]
        static IDatabaseProvider GetDatabaseProvider(SqlWrap sqlWrap, string partitionValues)
        {
            var dbConfig = SqlMapping.GetDatabase(sqlWrap.DbName);
            if ((dbConfig.DBType & sqlWrap.SupportDbType) == 0)
            {
                throw new ArgumentOutOfRangeException(string.Format("SqlWrap[{0}] 不支持类型为{1}的数据库[{2}]", sqlWrap.FullName, dbConfig.DBType, sqlWrap.DbName));
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
