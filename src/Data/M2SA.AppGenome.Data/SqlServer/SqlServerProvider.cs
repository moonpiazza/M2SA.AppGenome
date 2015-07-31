using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using M2SA.AppGenome.Data.SqlMap;
using M2SA.AppGenome.Logging;
using M2SA.AppGenome.Reflection;

namespace M2SA.AppGenome.Data.SqlServer
{
    /// <summary>
    /// 
    /// </summary>
    public class SqlServerProvider : DatabaseProviderBase
    {
        static readonly char ParameterToken = '@';

        #region IDatabaseProvider 成员

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="parameterValues"></param>
        /// <returns></returns>
        public override T ExecuteIdentity<T>(SqlWrap sql, IDictionary<string, object> parameterValues)
        {
            ArgumentAssertion.IsNotNull(sql, "sql");
            var identityValue = this.ExecuteScalar(sql, parameterValues);
            return identityValue.Convert<T>(default(T));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameterValues"></param>
        /// <returns></returns>
        public override int ExecuteNonQuery(SqlWrap sql, IDictionary<string, object> parameterValues)
        {
            ArgumentAssertion.IsNotNull(sql, "sql");

            Stopwatch stopwatch = null;
            var sqlProcessor = ObjectIOCFactory.GetSingleton<DataSettings>().SqlProcessor;
            if (sqlProcessor.EnableTrace) stopwatch = Stopwatch.StartNew();

            var sqlText = sql.SqlText;
            var commandType = sql.CommandType;
            if (CommandType.Text == commandType)
                sqlText = this.TransformSql(sql.SqlText, parameterValues);

            var result = SqlServerHelper.ExecuteNonQuery(this.ConnectionString, commandType, sqlText, sql.CommandTimeout, ConvertToDbParams(parameterValues));

            if (sqlProcessor.EnableTrace)
            {
                stopwatch.Stop();
                sqlProcessor.Log(LogLevel.Trace, string.Format("[{0}]ExecuteSql({1})\r\n\t{2}", stopwatch.Elapsed, sql.FullName, sqlText), null);
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameterValues"></param>
        /// <returns></returns>
        public override DataSet ExecuteDataSet(SqlWrap sql, IDictionary<string, object> parameterValues)
        {
            ArgumentAssertion.IsNotNull(sql, "sql");

            Stopwatch stopwatch = null;
            var sqlProcessor = ObjectIOCFactory.GetSingleton<DataSettings>().SqlProcessor;
            if (sqlProcessor.EnableTrace) stopwatch = Stopwatch.StartNew();

            var sqlText = sql.SqlText;
            var commandType = sql.CommandType;
            if (CommandType.Text == commandType)
                sqlText = this.TransformSql(sql.SqlText, parameterValues);

            var result = SqlServerHelper.ExecuteDataSet(this.ConnectionString, commandType, sqlText, sql.CommandTimeout, ConvertToDbParams(parameterValues));

            if (sqlProcessor.EnableTrace)
            {
                stopwatch.Stop();
                sqlProcessor.Log(LogLevel.Trace, string.Format("[{0}]ExecuteSql({1})\r\n\t{2}", stopwatch.Elapsed, sql.FullName, sqlText), null);
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameterValues"></param>
        /// <returns></returns>
        public override object ExecuteScalar(SqlWrap sql, IDictionary<string, object> parameterValues)
        {
            ArgumentAssertion.IsNotNull(sql, "sql");

            Stopwatch stopwatch = null;
            var sqlProcessor = ObjectIOCFactory.GetSingleton<DataSettings>().SqlProcessor;
            if (sqlProcessor.EnableTrace) stopwatch = Stopwatch.StartNew();

            var sqlText = sql.SqlText;
            var commandType = sql.CommandType;
            if (CommandType.Text == commandType)
                sqlText = this.TransformSql(sql.SqlText, parameterValues);

            var result = SqlServerHelper.ExecuteScalar(this.ConnectionString, commandType, sqlText, sql.CommandTimeout, ConvertToDbParams(parameterValues));
            if (result == DBNull.Value) result = null;

            if (sqlProcessor.EnableTrace)
            {
                stopwatch.Stop();
                sqlProcessor.Log(LogLevel.Trace, string.Format("[{0}]ExecuteSql({1})\r\n\t{2}", stopwatch.Elapsed, sql.FullName, sqlText), null);
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameterValues"></param>
        /// <returns></returns>
        public override DbDataReader ExecuteReader(SqlWrap sql, IDictionary<string, object> parameterValues)
        {
            ArgumentAssertion.IsNotNull(sql, "sql");

            Stopwatch stopwatch = null;
            var sqlProcessor = ObjectIOCFactory.GetSingleton<DataSettings>().SqlProcessor;
            if (sqlProcessor.EnableTrace) stopwatch = Stopwatch.StartNew();

            var sqlText = sql.SqlText;
            var commandType = sql.CommandType;
            if (CommandType.Text == commandType)
                sqlText = this.TransformSql(sql.SqlText, parameterValues);

            var result = SqlServerHelper.ExecuteReader(this.ConnectionString, commandType, sqlText, sql.CommandTimeout, ConvertToDbParams(parameterValues));

            if (sqlProcessor.EnableTrace)
            {
                stopwatch.Stop();
                sqlProcessor.Log(LogLevel.Trace, string.Format("[{0}]ExecuteSql({1})\r\n\t{2}", stopwatch.Elapsed, sql.FullName, sqlText), null);
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameterValues"></param>
        /// <param name="pagination"></param>
        /// <returns></returns>
        public override DataTable ExecutePaginationTable(SqlWrap sql, IDictionary<string, object> parameterValues, Pagination pagination)
        {
            ArgumentAssertion.IsNotNull(sql, "sql");
            ArgumentAssertion.IsNotNull(pagination, "pagination");

            Stopwatch stopwatch = null;
            var sqlProcessor = ObjectIOCFactory.GetSingleton<DataSettings>().SqlProcessor;
            if (sqlProcessor.EnableTrace) stopwatch = Stopwatch.StartNew();

            var sqlText = GetPaginationSql(sql, pagination);

            sqlText = this.TransformSql(sqlText, parameterValues);

            var dataSet = SqlServerHelper.ExecuteDataSet(this.ConnectionString, CommandType.Text, sqlText, sql.CommandTimeout, ConvertToDbParams(parameterValues));
            var totalCount = dataSet.Tables[0].Rows[0][0].Convert<int>();
            pagination.TotalCount = totalCount;

            if (sqlProcessor.EnableTrace)
            {
                stopwatch.Stop();
                sqlProcessor.Log(LogLevel.Trace, string.Format("[{0}]ExecuteSql({1})\r\n\t{2}", stopwatch.Elapsed, sql.FullName, sqlText), null);
            }

            return dataSet.Tables[1];
        }

        #endregion

        static SqlParameter[] ConvertToDbParams(IDictionary<string, object> parameterValues)
        {
            if (null == parameterValues || parameterValues.Count == 0)
            {
                return null;
            }

            var paramList = new SqlParameter[parameterValues.Count];
            var paramIndex = 0;
            foreach(var item in parameterValues)
            {
                var paramName = BuildParameterName(item.Key);
                var paramValue = FixDbParameterValue(item.Value);
                paramList[paramIndex] = new SqlParameter(paramName, paramValue);
                paramIndex++;
            }

            return paramList;
        }

        static object FixDbParameterValue(object paramValue)
        {
            if (null == paramValue) return paramValue;

            if (paramValue is DateTime)
            {
                var time = (DateTime)paramValue;
                if (time < Datestamp.ZeroTime)
                    paramValue = Datestamp.ZeroTime;
            }
            return paramValue;
        }

        static string BuildParameterName(string name)
        {
            if (name[0] != ParameterToken)
            {
                return name.Insert(0, new string(ParameterToken, 1));
            }
            return name;
        }

        static string GetPaginationSql(SqlWrap sql, Pagination pagination)
        {
            ArgumentAssertion.IsNotNull(sql, "sql");

            var paginationSql = PaginationSql.Parse(sql);
            
            var startIndex = (pagination.PageIndex - 1) * pagination.PageSize;
            var endIndex = pagination.PageIndex * pagination.PageSize;
            var whereExpression = string.IsNullOrEmpty(paginationSql.WhereExpression) ? "" : string.Concat("where ", paginationSql.WhereExpression);

            var sqlBuilder = new StringBuilder(1204);
            sqlBuilder.AppendFormat("select count(*) from {0} {1};\r\n", paginationSql.Tables, whereExpression);
            if (string.IsNullOrEmpty(sql.PrimaryKey))
            {
                sqlBuilder.AppendFormat("select {0} from (", paginationSql.Columns);
                sqlBuilder.AppendFormat("\r\n  select {0},row_number() over(order by {3}) _row_number from {1} {2}", paginationSql.Columns, paginationSql.Tables, whereExpression, paginationSql.OrderExpression);
                sqlBuilder.AppendFormat("\r\n ) _ZZZ where _row_number>{0} and _row_number<= {1} order by _row_number", startIndex, endIndex);
            }
            else
            {
                sqlBuilder.AppendFormat("select {0} from {1} _AAA, (", paginationSql.Columns, paginationSql.Tables);
                sqlBuilder.AppendFormat("\r\n  select {0} _zzzId,row_number() over(order by {3}) _row_number from {1} {2}", sql.PrimaryKey, paginationSql.Tables, whereExpression, paginationSql.OrderExpression);
                sqlBuilder.AppendFormat("\r\n ) _ZZZ where _row_number>{0} and _row_number<= {1} and _AAA.{2}=_ZZZ._zzzId order by _row_number", startIndex,endIndex,sql.PrimaryKey);
            }
            return sqlBuilder.ToString();
        }

        protected override string Escape(object val)
        {
            ArgumentAssertion.IsNotNull(val, "val");
            var str = val.ToString().Replace("'", "''");
            var isNumber = (val is int) || (val is long) || (val is decimal) || (val is short);

            if (false == isNumber)
                str = string.Format("'{0}'", str);
            return str;
        }
    }
}
