using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace M2SA.AppGenome.Logging
{
    /// <summary>
    /// 
    /// </summary>
    public static class ConstLogKeys
    {
        #region public static Fields

        /// <summary>
        /// 
        /// </summary>
        public static readonly char LabSeparator = '^';

        /// <summary>
        /// 
        /// </summary>
        public static readonly string HttpRequestKey = "HttpRequest";

        /// <summary>
        /// 
        /// </summary>
        public static readonly string StartKey = "Start";

        /// <summary>
        /// 
        /// </summary>
        public static readonly string EndKey = "End";

        /// <summary>
        /// 
        /// </summary>
        public static readonly string StepKey = "Step";

        /// <summary>
        /// 
        /// </summary>
        public static readonly string SpanKey = "Span";

        /// <summary>
        /// 
        /// </summary>
        public static readonly string NewLineKey = "NewLine";

        /// <summary>
        /// 
        /// </summary>
        public const string AppNameKey = "AppName";

        /// <summary>
        /// 
        /// </summary>
        public static readonly string ServerIPKey = "ServerIP";

        /// <summary>
        /// 
        /// </summary>
        public static readonly string SessionIdKey = "SessionId";

        /// <summary>
        /// 
        /// </summary>
        public static readonly string BizTypeKey = "BizType";

        /// <summary>
        /// 
        /// </summary>
        public static readonly string BIZLABSKEY = "BizLabs";

        /// <summary>
        /// 
        /// </summary>
        public static readonly string LOGLEVELKEY = "LogLevel";

        /// <summary>
        /// 
        /// </summary>
        public static readonly string MESSAGEKEY = "Message";

        /// <summary>
        /// 
        /// </summary>
        public static readonly string URIKEY = "URI";

        /// <summary>
        /// 
        /// </summary>
        public static readonly string LOGTIMEKEY = "LogTime";

        /// <summary>
        /// 
        /// </summary>
        public static readonly string WRITETIMEKEY = "WriteTime";

        /// <summary>
        /// 
        /// </summary>
        public static readonly string EXCEPTIONKEY = "Exception";

        /// <summary>
        /// 
        /// </summary>
        public static readonly string SYSINFOKEY = "SysInfo";

        /// <summary>
        /// 
        /// </summary>
        public static readonly string SYSINFOSERVERIPKEY = "SysInfo.ServerIp";

        /// <summary>
        /// 
        /// </summary>
        public static readonly string SYSINFOTOTALCPUKEY = "SysInfo.TotalCPU";

        /// <summary>
        /// 
        /// </summary>
        public static readonly string SYSINFOTOTALMEMORYKEY = "SysInfo.TotalMemory";

        /// <summary>
        /// 
        /// </summary>
        public static readonly string SYSINFOPROCESSNAMEKEY = "SysInfo.ProcessName";

        /// <summary>
        /// 
        /// </summary>
        public static readonly string SYSINFOPROCESSCPUKEY = "SysInfo.ProcessCPU";

        /// <summary>
        /// 
        /// </summary>
        public static readonly string SYSINFOPROCESSMEMORYKEY = "SysInfo.ProcessMemory";

        /// <summary>
        /// 
        /// </summary>
        public static readonly string SYSINFOTOTALTHREADCOUNTKEY = "SysInfo.TotalThreadCount";

        /// <summary>
        /// 
        /// </summary>
        public static readonly string SYSINFOTHREADNAMEKEY = "SysInfo.ThreadName";

        /// <summary>
        /// 
        /// </summary>
        public static readonly string SYSINFOTHREADIDKEY = "SysInfo.ThreadId";

        /// <summary>
        /// 
        /// </summary>
        public static readonly string SYSINFOTHREADPOOLKEY = "SysInfo.ThreadPool";


        /// <summary>
        /// 
        /// </summary>
        public static readonly string EXCEPTIONNAMESPACEKEY = "Exception.Namespace";

        /// <summary>
        /// 
        /// </summary>
        public const string EXCEPTIONNAMEKEY = "Exception.Name";

        /// <summary>
        /// 
        /// </summary>
        public static readonly string EXCEPTIONMESSAGEKEY = "Exception.Message";

        /// <summary>
        /// 
        /// </summary>
        public static readonly string EXCEPTIONSTACKTRACEKEY = "Exception.StackTrace";

        /// <summary>
        /// 
        /// </summary>
        public static readonly string EXCEPTIONTARGETSITEKEY = "Exception.TargetSite";

        /// <summary>
        /// 
        /// </summary>
        public static readonly string EXCEPTIONINNEREXCEPTIONKEY = "Exception.InnerException";


        /// <summary>
        /// 
        /// </summary>
        public static readonly string SESSIONCOLLECTIONKEY = "SESSIONCOLLECTION!@#";//会话信息的键标识
        #endregion
    }
}
