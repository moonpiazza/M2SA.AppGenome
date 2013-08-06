using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using M2SA.AppGenome.Configuration;
using M2SA.AppGenome.Diagnostics;
using M2SA.AppGenome.Threading;
using M2SA.AppGenome.Reflection;
using M2SA.AppGenome.Web;
using M2SA.AppGenome.Logging.Listeners;

namespace M2SA.AppGenome.Logging
{
    /// <summary>
    /// 
    /// </summary>
    public class Logger : ResolveObjectBase, ILog
    {
        /// <summary>
        /// 
        /// </summary>
        protected static readonly object SyncObject = new object();
        const string CreateEntryExceptionBizType = "~Logging.CreateEntryException~";
        const string OriginalLogLevel = "~Logging.OriginalLogLevel$~";

        string taskGroupName = null;

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
        public bool EnabledAsync
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public LevelConstraint LevelLimit
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public IDictionary<string, ListenerProxy> ListenerIndexes
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public List<IListener> Listeners
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public TimeSpan ProcessInterval
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public string ServerIP
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public Logger()
        {
            this.EnabledAsync = true;
            this.ProcessInterval = TimeSpan.FromMilliseconds(500);
            this.Listeners = new List<IListener>(2);
            this.LevelLimit = new LevelConstraint();

            this.ServerIP = SystemInfo.GetServerIP();
        }

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

        #region WriteMessage

        bool IsWrite(LogLevel level)
        {
            return (level >= this.LevelLimit.MinLevel && level <= this.LevelLimit.MaxLevel);            
        }

        bool IsWriteForListener(LogLevel level, ListenerProxy proxy)
        {
            return (level >= proxy.MinLevel && level <= proxy.MaxLevel);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entry"></param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        protected void WriteMessage(ILogEntry entry)
        {
            lock (SyncObject)
            {
                if (this.EnabledAsync)
                {
                    entry.WriteTime = DateTime.Now;
                    var syncCount = 0;
                    this.Listeners.ForEach(item => {
                        if (item.SupportAsync == false && this.IsWriteForListener(entry.LogLeveL, this.ListenerIndexes[item.Name]))
                        {
                        	try
                        	{
                            	item.WriteMessage(entry);
                        	}
                        	catch(Exception ex)
                        	{
                                EffectiveFileLogger.WriteException(ex);
                        	}
                            syncCount++;
                        }
                    });

                    if (this.Listeners.Count > syncCount)
                    {
                        this.WriteAsyncMessage(entry);
                    }
                }
                else
                {
                    entry.WriteTime = DateTime.Now;
                    this.Listeners.ForEach(item => {
                        if (this.IsWriteForListener(entry.LogLeveL, this.ListenerIndexes[item.Name]))
                        	try
                        	{
                            	item.WriteMessage(entry);
                        	}
                        	catch(Exception ex)
                        	{
                                EffectiveFileLogger.WriteException(ex);
                        	}                        	
                    });
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entry"></param>
        protected virtual void WriteAsyncMessage(ILogEntry entry)
        {
            AppInstance.GetTaskProcessor().Process<ILogEntry>(this.taskGroupName, entry);     
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entry"></param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        protected virtual void WriteAsyncMessageByThread(ILogEntry entry)
        {
            if (null == entry)
                throw new ArgumentNullException("entry");

            entry.WriteTime = DateTime.Now;
            this.Listeners.ForEach(item => {
                if (item.SupportAsync && this.IsWriteForListener(entry.LogLeveL, this.ListenerIndexes[item.Name]))
                    try
                    {
                        item.WriteMessage(entry);
                    }
                    catch (Exception ex)
                    {
                        EffectiveFileLogger.WriteException(ex);
                    }                    
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="level"></param>
        /// <param name="msg"></param>
        /// <param name="exception"></param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        public void WriteMessage(LogLevel level, object msg, Exception exception)
        {
            if (IsWrite(level) == false)
                return;

            try
            {
                var entry = CreateEntry(level, msg, exception);

                this.WriteMessage(entry);
            }
            catch (Exception ex)
            {
                this.WriteCreateEntryException(level, msg, exception, ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="originalLevel"></param>
        /// <param name="originalMessage"></param>
        /// <param name="originalException"></param>
        /// <param name="createEntryException"></param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        protected virtual void WriteCreateEntryException(LogLevel originalLevel, object originalMessage, Exception originalException, Exception createEntryException)
        {
            if (null == createEntryException)
                throw new ArgumentNullException("createEntryException");

            try
            {
                ILogEntry entry = new LogEntry();
                if (originalMessage != null)
                {
                    if (originalMessage is ILogEntry)
                    {
                        entry = (ILogEntry)originalMessage;
                    }
                    else if (originalMessage is IDictionary)
                    {
                        var items = (IDictionary)originalMessage;

                        foreach (DictionaryEntry item in items)
                        {
                            entry.ExtendInfo.Add(item.Key.ToString(), item.Value);
                        }
                    }
                    else
                    {
                        entry.Message = originalMessage.ToString();
                    }
                }
                entry.AppName = AppInstance.Config.AppName;
                entry.LogTime = DateTime.Now;
                entry.ServerIP = this.ServerIP;
                entry.BizType = CreateEntryExceptionBizType;
                if (originalException != null)
                {
                    entry.Exception = originalException;
                    entry.Message = string.Format("{0}\r\n[{1}]{2}", entry.Message, entry.BizType, originalException.Message);
                }
                else
                {
                    entry.Message = string.Format("{0}\r\n[{1}]{2}", entry.Message, entry.BizType, createEntryException.Message);
                }
                                
                entry.LogLeveL = LogLevel.Error;
                entry.ExtendInfo[OriginalLogLevel] = originalLevel;
                entry.ExtendInfo[CreateEntryExceptionBizType] = createEntryException;

                this.WriteMessage(entry);
            }
            catch
            {
                if (null != originalException)
                {
                    EffectiveFileLogger.WriteException(originalException);
                }
                EffectiveFileLogger.WriteException(createEntryException);
            }
        }

        #endregion

        #region CreateEntry

        /// <summary>
        /// 
        /// </summary>
        /// <param name="level"></param>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        /// <returns></returns>
        protected virtual ILogEntry CreateEntry(LogLevel level, object message, Exception exception)
        {
            if (message != null && message is Exception && exception == null)
            {
                return CreateEntry(level, null, exception);
            }

            ILogEntry entry = null;
            if (message == null)
            {
                entry = this.CreateEntry();
            }
            else if (message is ILogEntry)
            {
                entry = (ILogEntry)message;
            }
            else if (message is IDictionary)
            {
                entry = this.CreateEntry();
                var items = (IDictionary)message;
                var classAccessor = ClassAccessorRepository.GetClassAccessor(entry, AccessorType.Reflection);

                foreach (DictionaryEntry item in items)
                {
                    var key = item.Key.ToString();
                    if (classAccessor.PropertyAccessores.ContainsKey(key.ToLower()))
                        classAccessor.SetValue(entry, key, item.Value);
                    else
                        entry.ExtendInfo.Add(key, item.Value);
                }
            }
            else
            {
                entry = this.CreateEntry();
                entry.Message = message.ToString();
            }

            entry.LogLeveL = level;
            if (exception != null && entry.Exception == null)
                entry.Exception = exception;

            this.AppendExtendInfo(entry);

            return entry;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected virtual ILogEntry CreateEntry()
        {
            return new LogEntry();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        void AppendExtendInfo(ILogEntry entry)
        {
            if (string.IsNullOrEmpty(entry.AppName))
            {
                entry.AppName = AppInstance.Config.AppName;
            }
            if (entry.LogTime == DateTime.MinValue || entry.LogTime == DateTime.MaxValue)
            {
                entry.LogTime = DateTime.Now;
            }
            if (string.IsNullOrEmpty(entry.ServerIP))
            {
                entry.ServerIP = this.ServerIP;
            }

            if (string.IsNullOrEmpty(entry.Message) && entry.Exception != null)
            {
                entry.Message = entry.Exception.Message;
            }

            try
            {
                this.AppendSysInfo(entry);
                this.AppendBizLabs(entry);
            }
            catch (Exception ex)
            {
                this.WriteCreateEntryException(entry.LogLeveL, entry.Message, entry.Exception,ex);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        void AppendSysInfo(ILogEntry enrty)
        {
            if (string.IsNullOrEmpty(enrty.URI) && HttpContext.Current != null)
            {
                enrty.URI = HttpContext.Current.Request.FilePath;
            }

            if ((this.LevelLimit.SysInfoLimit & enrty.LogLeveL) != enrty.LogLeveL)
            {
                return;
            }

            if (enrty.SysInfo == null)
            {
                try
                {
                    enrty.SysInfo = SystemInfo.GetInfo();
                }
                catch (Exception ex)
                {
                    EffectiveFileLogger.WriteException(ex);
                }
            }

            var context = HttpContext.Current;
            if (context == null)
                return;

            if (enrty.ExtendInfo.ContainsKey(ConstLogKeys.HttpRequestKey) == false)
            {
                var requestInfo = new Dictionary<string, object>();
                if (context.Request.UrlReferrer != null)
                {
                    requestInfo.Add("Referrer", context.Request.UrlReferrer.ToString());
                }
                requestInfo.Add("URL", context.Request.Url.ToString());
                requestInfo.Add("RawUrl", context.Request.RawUrl);

                if (context.Request.Form.Count > 0)
                {
                    var formInfo = new Dictionary<string, object>(context.Request.Form.Count);
                    context.Request.Form.AllKeys.ToList<string>().ForEach(item => 
                    {
                        formInfo.Add(item, context.Request.Form[item]);
                    });
                    requestInfo.Add("FormParams", formInfo);
                }

                if (context.Request.Cookies.Count > 0)
                {
                    var cookiesInfo = new Dictionary<string, object>(context.Request.Cookies.Count);
                    context.Request.Cookies.AllKeys.ToList<string>().ForEach(item => {
                        var httpCookie = context.Request.Cookies[item];
                        var cookieInfo = new HttpCookieInfo(httpCookie);
                        var cookieKey = string.Format("{0}.{1}", httpCookie.Domain, item);
                        cookiesInfo[cookieKey] = cookieInfo;
                    });
                    requestInfo.Add("Cookies", cookiesInfo);
                }

                if (context.Request.ServerVariables.Count > 0)
                {
                    var serverInfo = new Dictionary<string, object>(context.Request.ServerVariables.Count);
                    context.Request.ServerVariables.AllKeys.ToList<string>().ForEach(item => {
                        var val = context.Request.ServerVariables[item];
                        if (string.IsNullOrEmpty(val) == false)
                        {
                            serverInfo[item] = context.Request.ServerVariables[item];
                        }
                    });
                    requestInfo.Add("ServerVariables", serverInfo);
                }

                try
                {
                    var browserInfo = new HttpBrowserInfo(context.Request.Browser);
                    requestInfo.Add("BrowserInfo", browserInfo);
                }
                catch (Exception ex)
                {
                    enrty.ExtendInfo.Add("BrowserInfo-Exception", ex);
                }

                enrty.ExtendInfo.Add(ConstLogKeys.HttpRequestKey, requestInfo);
            }
        }

        void AppendBizLabs(ILogEntry enrty)
        {
            var bizLabs = new List<string>(4);

            if (string.IsNullOrEmpty(enrty.BizLabs) == false)
            {
                var labs = enrty.BizLabs.Split(ConstLogKeys.LabSeparator);
                foreach(var lab in labs)
                    AppendBizLab(bizLabs, lab);
            }

            if (string.IsNullOrEmpty(enrty.BizType) == false)
            {
                AppendBizLab(bizLabs, enrty.BizType);
            }

            if (string.IsNullOrEmpty(enrty.URI) == false)
            {
                AppendBizLab(bizLabs, enrty.URI);
            }

            if (enrty.Exception != null)
            {
                AppendBizLab(bizLabs, enrty.Exception.GetType().FullName);
            }

            if (bizLabs.Count > 0)
            {
                enrty.BizLabs = string.Join(ConstLogKeys.LabSeparator.ToString(), bizLabs.ToArray());
            }
        }

        void AppendBizLab(List<string> bizLabs, string lab)
        {
            if (bizLabs.Contains(lab) == false)
            {
                bizLabs.Add(lab);
            }
        }

        #endregion
   
    
        /// <summary>
        /// 
        /// </summary>
        /// <param name="config"></param>
        public override void Initialize(IConfigNode config)
        {
            base.Initialize(config);            
            foreach(var key in this.ListenerIndexes.Keys)
            {
                var proxy = this.ListenerIndexes[key];
                if (proxy.Enabled)
                {
                    var listener = LogFactory.Instance.ListenerRespository.GetListener(proxy.Name);
                    this.Listeners.Add(listener);
                }
            }

            this.InitializeAsyncProcessor();
        }

        void InitializeAsyncProcessor()
        {
            if (this.EnabledAsync && (this is ISessionLog) == false)
            {
                this.taskGroupName = string.Format("{0}.{1}", AppConfig.LoggingKey, this.Name);
                AppInstance.GetTaskProcessor().RegisterAction<ILogEntry>(this.taskGroupName, this.WriteAsyncMessageByThread, this.ProcessInterval);
            }
        }

    }
}
