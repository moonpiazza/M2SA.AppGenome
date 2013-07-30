using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using M2SA.AppGenome.Diagnostics;

namespace M2SA.AppGenome.Logging
{
    /// <summary>
    /// 日志消息体接口
    /// </summary>
    public interface ILogEntry
    {
        /// <summary>
        /// 应用程序标识
        /// </summary>
        string AppName { get; set; }

        /// <summary>
        /// 会话标识
        /// </summary>
        string SessionId { get; set; }

        /// <summary>
        /// 会话标识
        /// </summary>
        string ServerIP { get; set; }

        /// <summary>
        /// 业务类型
        /// </summary>
        string BizType { get; set; }

        /// <summary>
        /// 业务标识
        /// </summary>
        string BizId
        {
            get;
            set;
        }

        /// <summary>
        /// 日志标签，可以有多个
        /// <see cref="ConstLogKeys.LabSeparator"/>
        /// </summary>
        string BizLabs { get; set; }

        /// <summary>
        /// 日志级别
        /// </summary>
        LogLevel LogLeveL { get; set; }

        /// <summary>
        /// 日志消息
        /// </summary>
        string Message { get; set; }

        /// <summary>
        /// 日志源URI
        /// </summary>
        string URI { get; set; }

        /// <summary>
        /// 日志时间
        /// </summary>
        DateTime LogTime { get; set; }

        /// <summary>
        /// 写日志时间
        /// </summary>
        DateTime WriteTime
        {
            get;
            set;
        }

        /// <summary>
        /// 扩展属性
        /// </summary>
        Dictionary<string, object> ExtendInfo
        {
            get;
        }

        /// <summary>
        /// 系统信息
        /// </summary>
        SystemInfo SysInfo { get; set; }
        /// <summary>
        /// 异常信息
        /// </summary>
        Exception Exception { get; set; }
    }

}
