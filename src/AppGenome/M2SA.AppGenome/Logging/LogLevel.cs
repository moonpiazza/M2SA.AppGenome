using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace M2SA.AppGenome.Logging
{
    /// <summary>
    /// 日志级别
    /// </summary>
    public enum LogLevel
    {
        /// <summary>
        /// 允许所有日志
        /// </summary>
        All = 0,

        /// <summary>
        /// 
        /// </summary>
        Debug = 1,

        /// <summary>
        /// 
        /// </summary>
        Trace = 2,

        /// <summary>
        /// 
        /// </summary>
        Info = 4,
        
        /// <summary>
        /// 
        /// </summary>
        Warn = 8,

        /// <summary>
        /// 
        /// </summary>
        Error = 16,
        
        /// <summary>
        /// 致命错误
        /// </summary>
        Fatal = 32,
        
        /// <summary>
        /// 禁止所有日志
        /// </summary>
        Off = 1024
    }
}
