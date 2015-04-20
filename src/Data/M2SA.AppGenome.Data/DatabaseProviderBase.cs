using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using M2SA.AppGenome.Data.SqlMap;

namespace M2SA.AppGenome.Data
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class DatabaseProviderBase : IDatabaseProvider
    {
        private IKeywordProcessor[] keywordProcessores = null;

        /// <summary>
        /// 
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        /// 
        /// </summary>
        protected DatabaseProviderBase()
        {
            this.keywordProcessores = new IKeywordProcessor[]
            {
                new IfPreprocessor(), 
                new LikeKeyProcessor(), 
                new InKeyProcessor()
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sqlText"></param>
        /// <param name="parameterValues"></param>
        /// <returns></returns>
        protected string TransformSql(string sqlText, IDictionary<string, object> parameterValues)
        {
            foreach (var processor in this.keywordProcessores)
            {
                var processResult = processor.Process(sqlText, parameterValues, Escape);
                if (processResult.IsMatch)
                {
                    sqlText = processResult.SqlExpression;
                    parameterValues = processResult.ParameterValues;
                }
            }

            return sqlText;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        protected abstract string Escape(object val);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="parameterValues"></param>
        /// <returns></returns>
        public abstract T ExecuteIdentity<T>(SqlWrap sql, IDictionary<string, object> parameterValues);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameterValues"></param>
        /// <returns></returns>
        public abstract int ExecuteNonQuery(SqlWrap sql, IDictionary<string, object> parameterValues);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameterValues"></param>
        /// <returns></returns>
        public abstract DataSet ExecuteDataSet(SqlWrap sql, IDictionary<string, object> parameterValues);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameterValues"></param>
        /// <returns></returns>
        public abstract object ExecuteScalar(SqlWrap sql, IDictionary<string, object> parameterValues);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameterValues"></param>
        /// <returns></returns>
        public abstract DbDataReader ExecuteReader(SqlWrap sql, IDictionary<string, object> parameterValues);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameterValues"></param>
        /// <param name="pagination"></param>
        /// <returns></returns>
        public abstract DataTable ExecutePaginationTable(SqlWrap sql, IDictionary<string, object> parameterValues, Pagination pagination);
    }
}
