using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Common;
using M2SA.AppGenome.Reflection;
using MySql.Data.MySqlClient;

namespace M2SA.AppGenome.Data.MySql
{
    /// <summary>
    /// 
    /// </summary>
    public class MySqlProvider : IDatabaseProvider
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
            var identity = MySqlHelper.ExecuteScalar(this.ConnectionString, commandText, ConvertToDBParams(parameterValues));
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
            return MySqlHelper.ExecuteNonQuery(this.ConnectionString, commandText, ConvertToDBParams(parameterValues));
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
            return MySqlHelper.ExecuteDataset(this.ConnectionString, commandText, ConvertToDBParams(parameterValues));
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
            var result = MySqlHelper.ExecuteScalar(this.ConnectionString, commandText, ConvertToDBParams(parameterValues));
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
            return MySqlHelper.ExecuteReader(this.ConnectionString, commandText, ConvertToDBParams(parameterValues));
        }

        #endregion

        static MySqlParameter[] ConvertToDBParams(IDictionary<string, object> parameterValues)
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
                paramList[paramIndex] = new MySqlParameter(paramName, item.Value);
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
