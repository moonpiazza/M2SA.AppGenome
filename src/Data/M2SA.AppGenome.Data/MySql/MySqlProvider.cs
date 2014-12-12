using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Common;
using M2SA.AppGenome.Data.SqlMap;
using M2SA.AppGenome.Reflection;
using MySql.Data.MySqlClient;
using MySqlHelper = M2SA.AppGenome.Data.MySql.MySqlHelper;

namespace M2SA.AppGenome.Data.MySql
{
    /// <summary>
    /// 
    /// </summary>
    public class MySqlProvider : IDatabaseProvider
    {
        static readonly char ParameterToken = '@';

        private IKeywordProcessor[] keywordProcessores = null;

        /// <summary>
        /// 
        /// </summary>
        public MySqlProvider()
        {
            this.keywordProcessores = new IKeywordProcessor[]
            {
                new LikeKeyProcessor(), 
                new InKeyProcessor()
            };
        }

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
            var identityValue = this.ExecuteScalar(sql, parameterValues);
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

            var sqlText = sql.SqlText;
            foreach (var processor in this.keywordProcessores)
            {
                var processResult = processor.Process(sqlText, parameterValues, Escape);
                if (processResult.IsMatch)
                {
                    sqlText = processResult.SqlExpression;
                    parameterValues = processResult.ParameterValues;
                }
            }
            return MySqlHelper.ExecuteNonQuery(this.ConnectionString, sqlText, sql.CommandTimeout, ConvertToDbParams(parameterValues));
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

            var sqlText = sql.SqlText;
            foreach (var processor in this.keywordProcessores)
            {
                var processResult = processor.Process(sqlText, parameterValues, Escape);
                if (processResult.IsMatch)
                {
                    sqlText = processResult.SqlExpression;
                    parameterValues = processResult.ParameterValues;
                }
            }
            return MySqlHelper.ExecuteDataSet(this.ConnectionString, sqlText, sql.CommandTimeout, ConvertToDbParams(parameterValues));
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

            var sqlText = sql.SqlText;
            foreach (var processor in this.keywordProcessores)
            {
                var processResult = processor.Process(sqlText, parameterValues, Escape);
                if (processResult.IsMatch)
                {
                    sqlText = processResult.SqlExpression;
                    parameterValues = processResult.ParameterValues;
                }
            }

            var result = MySqlHelper.ExecuteScalar(this.ConnectionString, sqlText, sql.CommandTimeout, ConvertToDbParams(parameterValues));
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

            var sqlText = sql.SqlText;
            foreach (var processor in this.keywordProcessores)
            {
                var processResult = processor.Process(sqlText, parameterValues, Escape);
                if (processResult.IsMatch)
                {
                    sqlText = processResult.SqlExpression;
                    parameterValues = processResult.ParameterValues;
                }
            }

            return MySqlHelper.ExecuteReader(this.ConnectionString, sqlText, sql.CommandTimeout, ConvertToDbParams(parameterValues));
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

            foreach (var processor in this.keywordProcessores)
            {
                var processResult = processor.Process(sqlText, parameterValues, Escape);
                if (processResult.IsMatch)
                {
                    sqlText = processResult.SqlExpression;
                    parameterValues = processResult.ParameterValues;
                }
            }

            var dataSet = MySqlHelper.ExecuteDataSet(this.ConnectionString, sqlText, sql.CommandTimeout, ConvertToDbParams(parameterValues));
            var totalCount = dataSet.Tables[0].Rows[0][0].Convert<int>();
            pagination.TotalCount = totalCount;
            return dataSet.Tables[1];
        }

        #endregion

        static MySqlParameter[] ConvertToDbParams(IDictionary<string, object> parameterValues)
        {
            if (null == parameterValues || parameterValues.Count == 0)
            {
                return null;
            }

            var paramList = new MySqlParameter[parameterValues.Count];
            var paramIndex = 0;
            foreach(var item in parameterValues)
            {
                var paramName = BuildParameterName(item.Key);
                var paramValue = FixDbParameterValue(item.Value);
                paramList[paramIndex] = new MySqlParameter(paramName, paramValue);
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

        static PaginationSql ParsePaginationSql(SqlWrap sql)
        {
            if (null == sql.PaginationSql)
            {
                lock (sql)
                {
                    if (null == sql.PaginationSql)
                    {
                        var parser = new PaginationSqlParser(sql.SqlText);
                        sql.PaginationSql = parser.Parse();
                    }
                }
            }
            return sql.PaginationSql;
        }

        static string GetPaginationSql(SqlWrap sql, Pagination pagination)
        {
            var paginationSql = ParsePaginationSql(sql);

            var startIndex = (pagination.PageIndex - 1) * pagination.PageSize;
            var whereExpression = string.IsNullOrEmpty(paginationSql.WhereExpression) ? "" : string.Concat("where ", paginationSql.WhereExpression);

            var sqlBuilder = new StringBuilder(1204);
            sqlBuilder.AppendFormat("select count(*) from {0} {1};\r\n", paginationSql.Tables, whereExpression);
            if (string.IsNullOrEmpty(sql.PrimaryKey))
            {
                sqlBuilder.AppendFormat("select {0} from {1} {2}", paginationSql.Columns, paginationSql.Tables, whereExpression);
                sqlBuilder.AppendFormat(" order by {0} limit {1},{2}", paginationSql.OrderExpression, startIndex, pagination.PageSize);
            }
            else
            {
                sqlBuilder.AppendFormat("select {0} from {1} _AAA, (", paginationSql.Columns, paginationSql.Tables);

                sqlBuilder.AppendFormat("\r\n  select {0} _zzzId from {1} {2}", sql.PrimaryKey, paginationSql.Tables, whereExpression);
                sqlBuilder.AppendFormat(" order by {0} limit {1},{2}", paginationSql.OrderExpression, startIndex, pagination.PageSize);

                sqlBuilder.AppendFormat("\r\n ) _ZZZ where _AAA.{0}=_ZZZ._zzzId", sql.PrimaryKey);
                sqlBuilder.AppendFormat(" order by {0}", paginationSql.OrderExpression);
            }
            return sqlBuilder.ToString();
        }

        static string Escape(object obj)
        {
            var str = obj.ToString().Replace(@"\", @"\\");
            str = str.Replace(@"'", @"\'");
            str = str.Replace(@"""", @"\""");
            var isNumber = (obj is int) || (obj is long) || (obj is decimal) || (obj is short);

            if (false == isNumber)
                str = string.Format("'{0}'", str);
            return str;
        }
    }
}
