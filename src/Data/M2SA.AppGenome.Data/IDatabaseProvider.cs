using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Common;

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
        /// <param name="commandText"></param>
        /// <param name="parameterValues"></param>
        /// <param name="commandType"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        T ExecuteIdentity<T>(string commandText, IDictionary<string, object> parameterValues, CommandType commandType, int timeout);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="parameterValues"></param>
        /// <param name="commandType"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        int ExecuteNonQuery(string commandText, IDictionary<string, object> parameterValues, CommandType commandType, int timeout);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="parameterValues"></param>
        /// <param name="commandType"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        DataSet ExecuteDataSet(string commandText, IDictionary<string, object> parameterValues, CommandType commandType, int timeout);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="parameterValues"></param>
        /// <param name="commandType"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        object ExecuteScalar(string commandText, IDictionary<string, object> parameterValues, CommandType commandType, int timeout);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="parameterValues"></param>
        /// <param name="commandType"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        DbDataReader ExecuteReader(string commandText, IDictionary<string, object> parameterValues, CommandType commandType, int timeout);

        //DataTable ExecuteTableForPage(string sqlText, IDictionary<string, object> parameterValues, CommandType commandType, string fileds, string orderbyPart);
    }
}
