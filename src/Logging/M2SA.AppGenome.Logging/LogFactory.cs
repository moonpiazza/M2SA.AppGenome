using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using M2SA.AppGenome.Configuration;
using M2SA.AppGenome.Reflection;
using M2SA.AppGenome.Threading;
using M2SA.AppGenome.Logging.Listeners;
using M2SA.AppGenome.Logging.Formatters;

namespace M2SA.AppGenome.Logging
{
    /// <summary>
    /// 
    /// </summary>
    public class LogFactory : ResolveFactoryBase<ILog>, ILogFactory
    {       
        /// <summary>
        /// 
        /// </summary>
        public static LogFactory Instance
        {
            get
            {
                return (LogFactory)ObjectIOCFactory.GetSingleton<ILogFactory>();
            }
        }

        string taskGroupName = null;
        IList<string> SessionLogIndexs = null;
        IDictionary<string, bool> LogSessionEnabledStateMap = null;
        IDictionary<string, IConfigNode> LogConfigMap = null;

        /// <summary>
        /// 
        /// </summary>
        protected override string ModuleKey
        {
            get
            {
                return AppConfig.LoggingKey;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public ListenerFactory ListenerRespository
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        private LogFactory()
        {
            this.SessionLogIndexs = new List<string>();
            this.LogSessionEnabledStateMap = new Dictionary<string, bool>();
            this.LogConfigMap = new Dictionary<string, IConfigNode>();
            this.ListenerRespository = new ListenerFactory();
            
            AppInstance.RegisterTypeAliasByModule<Logger>(AppConfig.LoggingKey);
            AppInstance.RegisterTypeAliasByModule<SessionLogger>(AppConfig.LoggingKey);
            AppInstance.RegisterTypeAliasByModule<SystemLogger>(AppConfig.LoggingKey);

            
            AppInstance.RegisterTypeAliasByModule<FileListener>(AppConfig.LoggingKey);
            AppInstance.RegisterTypeAliasByModule<ConsoleListener>(AppConfig.LoggingKey);
            AppInstance.RegisterTypeAliasByModule<QueueListener>(AppConfig.LoggingKey);


            AppInstance.RegisterTypeAliasByModule<TextFormatter>(AppConfig.LoggingKey);
            this.RegisterHandler();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public IConfigNode GetConfigInfo(string name)
        {
            return this.LoadConfigInfo(name);
        }

        /// <summary>
        /// 加载指定的配置信息
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        protected override IConfigNode LoadConfigInfo(string name)
        {
            if (this.LogConfigMap.ContainsKey(name) == false)
            {                
                var nodePath = string.Format("/loggers/logger[@name='{0}']", name);
                var configNode = AppInstance.GetConfigNode(this.ModuleKey, nodePath);
                if (null == configNode)
                {
                    var defaultCategory = this.GetDefaultCategory();
                    if (name == defaultCategory)
                        return null;
                    else
                        return this.LoadConfigInfo(defaultCategory);
                }

                if (string.IsNullOrEmpty(configNode.MetaType))
                {
                    configNode.MetaType = string.Format("{0}.{1}", AppConfig.LoggingKey, typeof(Logger).Name);
                }
                else if (configNode.MetaType.StartsWith(AppConfig.LoggingKey) == false)
                {
                    configNode.MetaType = string.Format("{0}.{1}", AppConfig.LoggingKey, configNode.MetaType);
                }

                this.LogConfigMap[name] = configNode;

                #region LogSessionMap

                var logType = TypeExtension.GetMapType(configNode.MetaType);
                if (null == logType)
                {
                    throw new ArgumentOutOfRangeException("name", name, "cannot find the type");
                }

                if (logType.GetInterface("ISessionLog") != null)
                {
                    this.LogSessionEnabledStateMap[name] = true;
                }
                else
                {
                    this.LogSessionEnabledStateMap[name] = false;
                }

                #endregion
            }
            return this.LogConfigMap[name];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:丢失范围之前释放对象")]
        protected override ILog ResolveObject(string name)
        {            
            if (this.LogConfigMap.ContainsKey(name) == false)
            {
                ILog log = null; 
                var defaultCategory = this.GetDefaultCategory();
                if (this.LogConfigMap.ContainsKey(defaultCategory))
                {
                    log = this.ResolveObject(defaultCategory);                    
                }
                else
                {
                    log = new SystemLogger();
                }

                log.Name = name;
                return log;
            }
            else
            {
                var obj = base.ResolveObject(name);
                obj.Name = name;
                if (obj is ISessionLog)
                {
                    var proxy = new SessionLoggerProxy(obj);
                    proxy.Name = name;
                    obj = proxy;
                }
                return obj;
            }
        }

        ILog GetLogInstance(string categoryName, string sessionKey)
        {
            ILog obj = null;
            var cacheKey = string.Format("{0}.{1}", categoryName.ToLower(), sessionKey);

            if (this.ObjectMap.ContainsKey(cacheKey))
            {
                obj = this.ObjectMap[cacheKey];                
            }
            else
            {
                lock (SyscObject)
                {
                    if (this.ObjectMap.ContainsKey(cacheKey))
                    {
                        obj = this.ObjectMap[cacheKey];
                    }
                    else
                    {
                        obj = this.ResolveObject(categoryName);
                        if (obj is ISessionLog)
                        {
                            (obj as ISessionLog).SessionId = sessionKey;
                        }
                        if (null != obj)
                        {
                            this.ObjectMap[cacheKey] = obj;
                            this.SessionLogIndexs.Add(cacheKey);
                        }
                    }
                }
            }

            return obj;
        }

        bool IsEnabledSession(string categoryName)
        {
            lock (SyscObject)
            {
                this.LoadConfigInfo(categoryName);
            }
            if (this.LogSessionEnabledStateMap.ContainsKey(categoryName))
                return this.LogSessionEnabledStateMap[categoryName];
            else
                return false;
        }

        #region ILogFactory 成员

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ILog GetLogger()
        {
            return this.GetLogger(this.GetDefaultCategory());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="categoryName"></param>
        /// <returns></returns>
        public ILog GetLogger(string categoryName)
        {
            if (this.IsEnabledSession(categoryName))
                return this.GetLogInstance(categoryName, Guid.NewGuid().ToString());
            else
                return this.GetLogInstance(categoryName, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sessionId"></param>
        /// <returns></returns>
        public ILog GetSessionLogger(string sessionId)
        {
            return this.GetSessionLogger(this.GetDefaultCategory(), sessionId);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="categoryName"></param>
        /// <param name="sessionId"></param>
        /// <returns></returns>
        public ILog GetSessionLogger(string categoryName, string sessionId)
        {
            if (this.IsEnabledSession(categoryName) == false)
            {
                throw new ArgumentException("此构造不支持会话模式,请选择合适的构造函数.");
            }

            return this.GetLogInstance(categoryName, sessionId);
        }

        #endregion

        #region SeesionLogger Builder Process

        void RegisterHandler()
        {
            this.taskGroupName = string.Format("{0}.sessionlog", AppConfig.LoggingKey);
            var timeAction = new TimeAction(this.taskGroupName, TimeSpan.FromSeconds(1), this.ProcessSessionLoggerHandler);
            AppInstance.GetTaskProcessor().RegisterAction(this.taskGroupName, timeAction);
        }

        void ProcessSessionLoggerHandler(DateTime now)
        {
            var sessionLoggerMap = new Dictionary<string, ISessionLog>(10);
            for (var i = 0; i < this.SessionLogIndexs.Count; i++)
            {
                var key = this.SessionLogIndexs[i];
                if (this.ObjectMap.ContainsKey(key))
                {
                    var itemValue = this.ObjectMap[key];
                    if (itemValue != null && itemValue is SessionLoggerProxy)
                    {
                        var proxy = itemValue as SessionLoggerProxy;
                        if (proxy.Target != null)
                        {
                            sessionLoggerMap[key] = proxy.TargetLog;
                        }
                    }
                }
            }

            foreach (var item in sessionLoggerMap)
            {
                var sessionLog = item.Value;
                if (sessionLog != null)
                    sessionLog.Flush(now);
            }
        }

        #endregion
    }

}
