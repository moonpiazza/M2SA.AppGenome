using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using M2SA.AppGenome.Data.SqlMap;
using M2SA.AppGenome.Reflection;

namespace M2SA.AppGenome.Data.SqlServer
{
    /// <summary>
    /// 
    /// </summary>
    public class SqlServerProvider : IDatabaseProvider
    {
        static readonly char ParameterToken = '@';

        #region IDatabaseProvider 成员

        /// <summary>
        /// 
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="parameterValues"></param>
        /// <returns></returns>
        public T ExecuteIdentity<T>(SqlWrap sql, IDictionary<string, object> parameterValues)
        {
            ArgumentAssertion.IsNotNull(sql, "sql");
            var identityValue = SqlServerHelper.ExecuteScalar(this.ConnectionString, sql.CommandType, sql.SqlText, ConvertToDbParams(parameterValues));
            return identityValue.Convert<T>(default(T));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameterValues"></param>
        /// <returns></returns>
        public int ExecuteNonQuery(SqlWrap sql, IDictionary<string, object> parameterValues)
        {
            ArgumentAssertion.IsNotNull(sql, "sql");
            return SqlServerHelper.ExecuteNonQuery(this.ConnectionString, sql.CommandType, sql.SqlText, ConvertToDbParams(parameterValues));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameterValues"></param>
        /// <returns></returns>
        public DataSet ExecuteDataSet(SqlWrap sql, IDictionary<string, object> parameterValues)
        {
            ArgumentAssertion.IsNotNull(sql, "sql");
            return SqlServerHelper.ExecuteDataSet(this.ConnectionString, sql.CommandType, sql.SqlText, ConvertToDbParams(parameterValues));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameterValues"></param>
        /// <returns></returns>
        public object ExecuteScalar(SqlWrap sql, IDictionary<string, object> parameterValues)
        {
            ArgumentAssertion.IsNotNull(sql, "sql");
            var result = SqlServerHelper.ExecuteScalar(this.ConnectionString, sql.CommandType, sql.SqlText, ConvertToDbParams(parameterValues));
            if (result == DBNull.Value) result = null;
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameterValues"></param>
        /// <returns></returns>
        public DbDataReader ExecuteReader(SqlWrap sql, IDictionary<string, object> parameterValues)
        {
            ArgumentAssertion.IsNotNull(sql, "sql");
            return SqlServerHelper.ExecuteReader(this.ConnectionString, sql.CommandType, sql.SqlText, ConvertToDbParams(parameterValues));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameterValues"></param>
        /// <param name="pagination"></param>
        /// <returns></returns>
        public DataTable ExecutePaginationTable(SqlWrap sql, IDictionary<string, object> parameterValues, Pagination pagination)
        {
            ArgumentAssertion.IsNotNull(sql, "sql");
            ArgumentAssertion.IsNotNull(pagination, "pagination");

            var sqlText = GetPaginationSql(sql, pagination);
            Console.WriteLine(sqlText);
            var dataSet = SqlServerHelper.ExecuteDataSet(this.ConnectionString, CommandType.Text, sqlText, ConvertToDbParams(parameterValues));
            var totalCount = dataSet.Tables[0].Rows[0][0].Convert<int>();
            pagination.TotalCount = totalCount;
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
                paramList[paramIndex] = new SqlParameter(paramName, item.Value);
                paramIndex++;
            }

            return paramList;
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

            var parser = new PaginationSqlParser(sql.SqlText);
            parser.Parse();
            
            var startIndex = (pagination.PageIndex - 1) * pagination.PageSize;
            var endIndex = pagination.PageIndex * pagination.PageSize;
            var whereExpression = string.IsNullOrEmpty(parser.WhereExpression) ? "" : string.Concat("where ", parser.WhereExpression);

            var sqlBuilder = new StringBuilder(1204);
            sqlBuilder.AppendFormat("select count(*) from {0} {1};\r\n", parser.Tables, whereExpression);
            if (string.IsNullOrEmpty(sql.PrimaryKey))
            {
                sqlBuilder.AppendFormat("select {0} from (", parser.Columns);
                sqlBuilder.AppendFormat("\r\n  select {0},row_number() over(order by {3}) _row_number from {1} {2}", parser.Columns, parser.Tables, whereExpression, parser.OrderExpression);
                sqlBuilder.AppendFormat("\r\n ) _ZZZ where _row_number>{0} and _row_number<= {1} order by _row_number", startIndex, endIndex);
            }
            else
            {
                sqlBuilder.AppendFormat("select {0} from {1} _AAA, (", parser.Columns, parser.Tables);
                sqlBuilder.AppendFormat("\r\n  select {0} _zzzId,row_number() over(order by {3}) _row_number from {1} {2}", sql.PrimaryKey, parser.Tables, whereExpression, parser.OrderExpression);
                sqlBuilder.AppendFormat("\r\n ) _ZZZ where _row_number>{0} and _row_number<= {1} and _AAA.{2}=_ZZZ._zzzId order by _row_number", startIndex,endIndex,sql.PrimaryKey);
            }
            return sqlBuilder.ToString();
        }
    }
}
