using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using M2SA.AppGenome.Configuration;
using M2SA.AppGenome.Reflection;

namespace M2SA.AppGenome.ExceptionHandling
{
    /// <summary>
    /// 
    /// </summary>
    public class ReplaceHandler : IExceptionHandler
    {
        static readonly string MesssageKey = "@Message";

        /// <summary>
        /// 
        /// </summary>
        public string ReplaceMessage
        {
            get;
            private set;
        }

        /// <summary>
        /// 
        /// </summary>
        public string ReplaceType
        {
            get;
            private set;
        }

        #region IExceptionHandler 成员

        /// <summary>
        /// 
        /// </summary>
        /// <param name="exception"></param>
        /// <param name="handlingInstanceId"></param>
        /// <param name="bizInfo"></param>
        /// <returns></returns>
        public Exception HandleException(Exception exception, Guid handlingInstanceId, IDictionary bizInfo)
        {
            if (null == exception)
                return null;

            var msg = this.ReplaceMessage;
            if (string.IsNullOrEmpty(msg))
                msg = exception.Message;
            else if (msg.Contains(MesssageKey))
                msg = msg.Replace(MesssageKey, exception.Message);

            if (string.IsNullOrEmpty(this.ReplaceType))
            {
                throw new ConfigException("node define the ReplaceType");
            }

            var targetExceptionType = TypeExtension.GetMapType(this.ReplaceType);
            if (null == targetExceptionType)
            {
                throw new ConfigException(string.Format("cannot find the ExceptionType :{0}", this.ReplaceType));
            }

            object[] extraParameters = new object[1] { msg };
            return (Exception)Activator.CreateInstance(targetExceptionType, extraParameters);
        }

        #endregion
    }
}
