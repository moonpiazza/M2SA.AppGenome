using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

namespace M2SA.AppGenome.Data.MySql
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class MySqlHelper
    {
        #region ExecuteNonQuery

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="commandText"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="parms"></param>
        /// <returns></returns>
        public static int ExecuteNonQuery(string connectionString, string commandText, int commandTimeout, params MySqlParameter[] parms)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                return ExecuteNonQuery(connection, commandText, commandTimeout, parms);
            }
        }
       
        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="commandText"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="commandParameters"></param>
        /// <returns></returns>
        public static int ExecuteNonQuery(MySqlConnection connection, string commandText, int commandTimeout, params MySqlParameter[] commandParameters)
        {
            using (var cmd = PrepareCommand(connection, commandText, commandTimeout))
            {
                if (commandParameters != null)
                {
                    foreach (MySqlParameter p in commandParameters)
                        cmd.Parameters.Add(p);
                }

                int result = cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();
                return result;
            }
        }

        #endregion

        #region ExecuteDataSet

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="commandText"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="commandParameters"></param>
        /// <returns></returns>
        public static DataSet ExecuteDataSet(string connectionString, string commandText, int commandTimeout, params MySqlParameter[] commandParameters)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                return ExecuteDataSet(connection, commandText, commandTimeout, commandParameters);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="commandText"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="commandParameters"></param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:丢失范围之前释放对象")]
        public static DataSet ExecuteDataSet(MySqlConnection connection, string commandText, int commandTimeout, params MySqlParameter[] commandParameters)
        {
            using (var cmd = PrepareCommand(connection, commandText, commandTimeout))
            {

                if (commandParameters != null)
                {
                    foreach (MySqlParameter p in commandParameters)
                        cmd.Parameters.Add(p);
                }

                using (var adapter = new MySqlDataAdapter(cmd))
                {
                    var dataSet = new DataSet();

                    adapter.Fill(dataSet);
                    cmd.Parameters.Clear();
                    return dataSet;
                }
            }
        }

        #endregion

        #region ExecuteScalar

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="commandText"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="commandParameters"></param>
        /// <returns></returns>
        public static object ExecuteScalar(string connectionString, string commandText, int commandTimeout, params MySqlParameter[] commandParameters)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                return ExecuteScalar(connection, commandText, commandTimeout, commandParameters);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="commandText"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="commandParameters"></param>
        /// <returns></returns>
        public static object ExecuteScalar(MySqlConnection connection, string commandText, int commandTimeout, params MySqlParameter[] commandParameters)
        {
            using (var cmd = PrepareCommand(connection, commandText, commandTimeout))
            {
                if (commandParameters != null)
                {
                    foreach (MySqlParameter p in commandParameters)
                        cmd.Parameters.Add(p);
                }

                object result = cmd.ExecuteScalar();
                cmd.Parameters.Clear();
                return result;                
            }
        }

        #endregion

        #region ExecuteReader

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="commandText"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="commandParameters"></param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:丢失范围之前释放对象")]
        public static MySqlDataReader ExecuteReader(string connectionString, string commandText, int commandTimeout, params MySqlParameter[] commandParameters)
        {
            var connection = new MySqlConnection(connectionString);
            connection.Open();
            return ExecuteReader(connection, commandText, commandTimeout, commandParameters);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="commandText"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="commandParameters"></param>
        /// <returns></returns>
        public static MySqlDataReader ExecuteReader(MySqlConnection connection, string commandText, int commandTimeout, MySqlParameter[] commandParameters)
        {
            var cmd = PrepareCommand(connection, commandText, commandTimeout);

            if (commandParameters != null)
            {
                foreach (MySqlParameter p in commandParameters)
                    cmd.Parameters.Add(p);
            }

            var dataReader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
            cmd.Parameters.Clear();

            return dataReader;
        }

        #endregion

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:检查 SQL 查询是否存在安全漏洞")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:丢失范围之前释放对象")]
        private static MySqlCommand PrepareCommand(MySqlConnection connection, string commandText, int commandTimeout)
        {
            var cmd = new MySqlCommand();
            cmd.Connection = connection;
            cmd.CommandText = commandText;
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = commandTimeout;

            return cmd;
        }
    }
}
