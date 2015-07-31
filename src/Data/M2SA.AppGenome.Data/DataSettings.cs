using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using M2SA.AppGenome.Configuration;
using M2SA.AppGenome.ExceptionHandling;
using M2SA.AppGenome.Logging;

namespace M2SA.AppGenome.Data
{
    /// <summary>
    /// 
    /// </summary>
    public class DataSettings : ResolveObjectBase
    {
        /// <summary>
        /// 
        /// </summary>
        public SqlProcessor SqlProcessor { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="config"></param>
        public override void Initialize(IConfigNode config)
        {
            base.Initialize(config);
            if (null == this.SqlProcessor)
            {
                this.SqlProcessor = new SqlProcessor();
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class SqlProcessor
    {
        /// <summary>
        /// 等待命令执行的时间（以秒为单位）。默认值为 30 秒。 值 0 指示无限制，应尽量避免值 0，否则会无限期地等待执行命令。
        /// </summary>
        public int CommandTimeout { get; set; }

        /// <summary>
        /// 
        /// 
        /// </summary>
        public bool EnableTrace { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string LogCategory { get; set; }
        

        /// <summary>
        /// 
        /// </summary>
        public SqlProcessor()
        {
            this.CommandTimeout = 30; 
            this.EnableTrace = false;
            this.LogCategory = "sql";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="level"></param>
        /// <param name="msg"></param>
        /// <param name="exception"></param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        public void Log(LogLevel level, object msg, Exception exception)
        {
            if (false == this.EnableTrace) return;
            try
            {
                LogManager.GetLogger(this.LogCategory).WriteMessage(level, msg, exception);
            }
            catch (Exception ex)
            {
                EffectiveFileLogger.WriteException(exception);
                EffectiveFileLogger.WriteException(ex);
            }
        }
    }
}
