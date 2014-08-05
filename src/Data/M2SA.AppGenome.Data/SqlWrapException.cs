using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;

namespace M2SA.AppGenome.Data
{

    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class SqlWrapException : DbException
    {
        static readonly Regex UserRegex =new Regex(@";\s*(?:user|user id)\s*=([^;]*)", RegexOptions.IgnoreCase);
        static readonly Regex PasswordRegex =new Regex(@";\s*password\s*=([^;""]*)", RegexOptions.IgnoreCase);

        private static string GetSecureMessage(string message)
        {
            if (string.IsNullOrEmpty(message)) return message;

            var match = UserRegex.Match(message);
            if (match.Success)
            {
                message = message.Replace(match.Groups[1].Value, "******");
            }

            match = PasswordRegex.Match(message);
            if (match.Success)
            {
                message = message.Replace(match.Groups[1].Value, "******");
            }

            return message;
        }

        /// <summary>
        /// 
        /// </summary>
        public SqlWrapException()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public SqlWrapException(string message) : base (GetSecureMessage(message))
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public SqlWrapException(string message, Exception innerException) : base(GetSecureMessage(message), innerException)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="errorCode"></param>
        public SqlWrapException(string message, int errorCode) : base(GetSecureMessage(message), errorCode)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="innerException"></param>
        /// <param name="sqlName"></param>
        /// <param name="parameterValues"></param>
        public SqlWrapException(Exception innerException, string sqlName, IDictionary<string, object> parameterValues)
            : base(null == innerException ? "" : string.Format("[{0}]{1}", sqlName, GetSecureMessage(innerException.Message)), innerException)
        {
            base.Data.Add("M2SA.AppGenome.Data:SqlWrap", sqlName);
            if(null != parameterValues && parameterValues.Count > 0)
                base.Data.Add("M2SA.AppGenome.Data:Params", parameterValues);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected SqlWrapException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
