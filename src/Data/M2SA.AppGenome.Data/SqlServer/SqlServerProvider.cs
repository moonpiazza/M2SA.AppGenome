using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
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
        /// <param name="commandText"></param>
        /// <param name="parameterValues"></param>
        /// <param name="commandType"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public T ExecuteIdentity<T>(string commandText, IDictionary<string, object> parameterValues, CommandType commandType, int timeout)
        {
            var identity = SqlServerHelper.ExecuteScalar(this.ConnectionString, commandType, commandText, ConvertToDBParams(parameterValues));
            if (identity == DBNull.Value)
                return default(T);
            else
                return identity.Convert<T>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="parameterValues"></param>
        /// <param name="commandType"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public int ExecuteNonQuery(string commandText, IDictionary<string, object> parameterValues, CommandType commandType, int timeout)
        {
            return SqlServerHelper.ExecuteNonQuery(this.ConnectionString, commandType, commandText, ConvertToDBParams(parameterValues));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="parameterValues"></param>
        /// <param name="commandType"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public DataSet ExecuteDataSet(string commandText, IDictionary<string, object> parameterValues, System.Data.CommandType commandType, int timeout)
        {
            return SqlServerHelper.ExecuteDataSet(this.ConnectionString, commandType, commandText, ConvertToDBParams(parameterValues));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="parameterValues"></param>
        /// <param name="commandType"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public object ExecuteScalar(string commandText, IDictionary<string, object> parameterValues, CommandType commandType, int timeout)
        {
            var result = SqlServerHelper.ExecuteScalar(this.ConnectionString, commandType, commandText, ConvertToDBParams(parameterValues));
            if (result == DBNull.Value) result = null;
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="parameterValues"></param>
        /// <param name="commandType"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public DbDataReader ExecuteReader(string commandText, IDictionary<string, object> parameterValues, CommandType commandType, int timeout)
        {
            return SqlServerHelper.ExecuteReader(this.ConnectionString, commandType, commandText, ConvertToDBParams(parameterValues));
        }

        #endregion

        static SqlParameter[] ConvertToDBParams(IDictionary<string, object> parameterValues)
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
    }
}
