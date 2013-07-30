using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using M2SA.AppGenome.Diagnostics;

namespace M2SA.AppGenome.Logging
{    
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class LogEntry : ILogEntry
    {

        #region ILogEntry 成员

        /// <summary>
        /// 应用程序标识
        /// </summary>
        public string AppName
        {
            get;
            set;
        }

        /// <summary>
        /// 会话标识
        /// </summary>
        public string SessionId
        {
            get;
            set;
        }

        /// <summary>
        /// 会话标识
        /// </summary>
        public string ServerIP
        {
            get;
            set;
        }

        /// <summary>
        /// 业务类型
        /// </summary>
        public string BizType
        {
            get;
            set;
        }

        /// <summary>
        /// 业务标识
        /// </summary>
        public string BizId
        {
            get;
            set;
        }

        /// <summary>
        /// 日志标签，可以有多个
        /// <see cref="ConstLogKeys.LabSeparator"/>
        /// </summary>
        public string BizLabs
        {
            get;
            set;
        }

        /// <summary>
        /// 日志级别
        /// </summary>
        public virtual LogLevel LogLeveL
        {
            get;
            set;
        }

        /// <summary>
        /// 日志消息
        /// </summary>
        public string Message
        {
            get;
            set;
        }

        /// <summary>
        /// 日志源URI
        /// </summary>
        public string URI
        {
            get;
            set;
        }

        /// <summary>
        /// 日志时间
        /// </summary>
        public DateTime LogTime
        {
            get;
            set;
        }

        /// <summary>
        /// 写日志时间
        /// </summary>
        public DateTime WriteTime
        {
            get;
            set;
        }

        /// <summary>
        /// 异常信息
        /// </summary>
        public Exception Exception
        {
            get;
            set;
        }

        /// <summary>
        /// 扩展属性
        /// </summary>
        public Dictionary<string, object> ExtendInfo
        {
            get;
            private set;
        }

        /// <summary>
        /// 系统信息
        /// </summary>
        public SystemInfo SysInfo
        {
            get;
            set;
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        public LogEntry()
        {
            this.ExtendInfo = new Dictionary<string, object>();            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public T GetExtendValue<T>(string key)
        {
            if (this.ExtendInfo.ContainsKey(key))
            {
                return (T)this.ExtendInfo[key];
            }
            else
                return default(T);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="val"></param>
        public void SetExtendValue<T>(string key, T val)
        {
            this.ExtendInfo[key] = val;
        }
    }
}
