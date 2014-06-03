using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Common;
using M2SA.AppGenome.Data.SqlMap;

namespace M2SA.AppGenome.Data
{
    /// <summary>
    /// 
    /// </summary>
    public interface IDatabaseProvider
    {
        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        string ConnectionString { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameterValues"></param>
        /// <returns></returns>
        T ExecuteIdentity<T>(SqlWrap sql, IDictionary<string, object> parameterValues);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameterValues"></param>
        /// <returns></returns>
        int ExecuteNonQuery(SqlWrap sql, IDictionary<string, object> parameterValues);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameterValues"></param>
        /// <returns></returns>
        DataSet ExecuteDataSet(SqlWrap sql, IDictionary<string, object> parameterValues);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameterValues"></param>
        /// <returns></returns>
        object ExecuteScalar(SqlWrap sql, IDictionary<string, object> parameterValues);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameterValues"></param>
        /// <returns></returns>
        DbDataReader ExecuteReader(SqlWrap sql, IDictionary<string, object> parameterValues);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameterValues"></param>
        /// <param name="pagination"></param>
        /// <returns></returns>
        DataTable ExecutePaginationTable(SqlWrap sql, IDictionary<string, object> parameterValues, Pagination pagination);
    }
}
