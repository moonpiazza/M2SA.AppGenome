using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using M2SA.AppGenome.Configuration;
using M2SA.AppGenome.Logging;

namespace M2SA.AppGenome
{
    /// <summary>
    /// 
    /// </summary>
    public interface ILog : IResolveObject
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="level"></param>
        /// <param name="msg"></param>
        /// <param name="exception"></param>
        void WriteMessage(LogLevel level, object msg, Exception exception);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message">消息类型：简单值对象, IDictionary[string, object], ILogEntry实现</param>
        void Debug(object message);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        void Debug(string format, params object[] args);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message">消息类型：简单值对象, IDictionary[string, object], ILogEntry实现</param>
        void Trace(object message);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        void Trace(string format, params object[] args);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message">消息类型：简单值对象, IDictionary[string, object], ILogEntry实现</param>
        void Info(object message);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        void Info(string format, params object[] args);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message">消息类型：简单值对象, IDictionary[string, object], ILogEntry实现</param>
        void Warn(object message);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        void Warn(string format, params object[] args);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="exception"></param>
        /// <param name="message">消息类型：简单值对象, IDictionary[string, object], ILogEntry实现</param>
        void Warn(Exception exception, object message);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message">消息类型：简单值对象, IDictionary[string, object], ILogEntry实现</param>
        void Error(object message);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        void Error(string format, params object[] args);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="exception"></param>
        /// <param name="message">消息类型：简单值对象, IDictionary[string, object], ILogEntry实现</param>
        void Error(Exception exception, object message);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message">消息类型：简单值对象, IDictionary[string, object], ILogEntry实现</param>
        void Fatal(object message);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        void Fatal(string format, params object[] args);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="exception"></param>
        /// <param name="message">消息类型：简单值对象, IDictionary[string, object], ILogEntry实现</param>
        void Fatal(Exception exception, object message);
    }
}
