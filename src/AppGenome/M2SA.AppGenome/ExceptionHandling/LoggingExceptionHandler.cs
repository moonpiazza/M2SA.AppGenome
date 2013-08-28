using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using M2SA.AppGenome.Logging;
using M2SA.AppGenome.Reflection;

namespace M2SA.AppGenome.ExceptionHandling
{
    /// <summary>
    /// 
    /// </summary>
    public class LoggingExceptionHandler : IExceptionHandler
    {
        /// <summary>
        /// 
        /// </summary>
        public LogLevel LogLevel
        {
            get;
            internal set;
        }

        /// <summary>
        /// 
        /// </summary>
        public string LogCategory
        {
            get;
            internal set;
        }

        #region IExceptionHandler 成员

        /// <summary>
        /// 
        /// </summary>
        /// <param name="exception"></param>
        /// <param name="handlingInstanceId"></param>
        /// <param name="bizInfo"></param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        public Exception HandleException(Exception exception, Guid handlingInstanceId, IDictionary bizInfo)
        {
            try
            {
                LogManager.GetLogger(this.LogCategory).WriteMessage(this.LogLevel, bizInfo, exception);
            }
            catch (Exception ex)
            {
                EffectiveFileLogger.WriteException(exception);
                EffectiveFileLogger.WriteException(ex);
            }
            return exception;
        }

        #endregion
    }
}
