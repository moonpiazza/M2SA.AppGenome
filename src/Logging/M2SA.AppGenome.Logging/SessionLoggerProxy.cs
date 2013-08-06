using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using M2SA.AppGenome.Configuration;

namespace M2SA.AppGenome.Logging
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class SessionLoggerProxy : WeakReference, ISessionLog
    {
        string sessionId = null;

        /// <summary>
        /// 
        /// </summary>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        public SessionLoggerProxy(ILog logger)
            : base(logger)
        { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="trackResurrection"></param>
        public SessionLoggerProxy(ILog logger, bool trackResurrection)
            : base(logger, trackResurrection)
        { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected SessionLoggerProxy(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            if (null == info)
                throw new ArgumentNullException("info");

            this.Name = info.GetString("Name");
            this.sessionId = info.GetString("SessionId");
        }

        /// <summary>
        /// 
        /// </summary>
        public ISessionLog TargetLog
        {
            get
            {
                var log = base.Target as ISessionLog;
                if (log == null)
                {
                    var configNode = LogFactory.Instance.GetConfigInfo(this.Name);
                    log = (ISessionLog)typeof(ISessionLog).BuildObject();
                    log.Initialize(configNode);
                    log.SessionId = this.sessionId;
                    this.Target = log;
                }
                return log;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (null == info)
                throw new ArgumentNullException("info");

            base.GetObjectData(info, context);
            info.AddValue("Name", this.Name);
            info.AddValue("SessionId", this.sessionId);
        }

        #region ISessionLog 成员

        /// <summary>
        /// 
        /// </summary>
        public string SessionId
        {
            get
            {
                return this.sessionId;
            }
            set
            {
                this.sessionId = value;
                this.TargetLog.SessionId = this.sessionId;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="level"></param>
        /// <param name="msg"></param>
        /// <param name="exception"></param>
        public void WriteMessage(LogLevel level, object msg, Exception exception)
        {
            var log = this.TargetLog;
            if (null == log)
                return;

            log.WriteMessage(level, msg, exception);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="now"></param>
        public void Flush(DateTime now)
        {
            //empty action
        }

        #endregion

        #region ILog 成员

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public void Debug(object message)
        {
            this.WriteMessage(LogLevel.Debug, message, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        public void Debug(string format, params object[] args)
        {
            this.WriteMessage(LogLevel.Debug, string.Format(format, args), null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public void Trace(object message)
        {
            this.WriteMessage(LogLevel.Trace, message, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        public void Trace(string format, params object[] args)
        {
            this.WriteMessage(LogLevel.Trace, string.Format(format, args), null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public void Info(object message)
        {
            this.WriteMessage(LogLevel.Info, message, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        public void Info(string format, params object[] args)
        {
            this.WriteMessage(LogLevel.Info, string.Format(format, args), null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public void Warn(object message)
        {
            this.WriteMessage(LogLevel.Warn, message, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        public void Warn(string format, params object[] args)
        {
            this.WriteMessage(LogLevel.Warn, string.Format(format, args), null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="exception"></param>
        /// <param name="message"></param>
        public void Warn(Exception exception, object message)
        {
            this.WriteMessage(LogLevel.Warn, message, exception);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public void Error(object message)
        {
            this.WriteMessage(LogLevel.Error, message, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        public void Error(string format, params object[] args)
        {
            this.WriteMessage(LogLevel.Error, string.Format(format, args), null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="exception"></param>
        /// <param name="message"></param>
        public void Error(Exception exception, object message)
        {
            this.WriteMessage(LogLevel.Error, message, exception);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public void Fatal(object message)
        {
            this.WriteMessage(LogLevel.Fatal, message, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        public void Fatal(string format, params object[] args)
        {
            this.WriteMessage(LogLevel.Fatal, string.Format(format, args), null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="exception"></param>
        /// <param name="message"></param>
        public void Fatal(Exception exception, object message)
        {
            this.WriteMessage(LogLevel.Fatal, message, exception);
        }

        #endregion

        #region IDisposable 

        /// <summary>
        /// 
        /// </summary>
        ~SessionLoggerProxy() 
        { 
            Dispose(false); 
        }

        void Dispose(bool disposing)
        {
            if (disposing == true && this.Target != null)
            {
                (this.Target as ISessionLog).Dispose();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            Dispose(true); 
        }
        #endregion

        #region IResolveObject 成员

        /// <summary>
        /// 
        /// </summary>
        /// <param name="config"></param>
        public void Initialize(IConfigNode config)
        {
            // empty action
        }

        #endregion
    }
}
