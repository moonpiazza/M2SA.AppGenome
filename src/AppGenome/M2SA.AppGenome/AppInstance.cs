using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;
using System.Configuration;
using M2SA.AppGenome.AppHub;
using M2SA.AppGenome.Configuration;
using M2SA.AppGenome.ExceptionHandling;
using M2SA.AppGenome.Logging;
using M2SA.AppGenome.Threading;
using M2SA.AppGenome.Reflection;
using M2SA.AppGenome.Web;

namespace M2SA.AppGenome
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class AppInstance : IExtensionApplication
    {
        #region Static Members
        
        static readonly AppConfig appConfig;

        /// <summary>
        /// 
        /// </summary>
        public static AppConfig Config
        {
            get
            {
                return appConfig;
            }
        }

        static AppInstance()
        {
            var config = new AppConfig();
            var configNode = AppInstance.GetConfigNode(AppConfig.AppBaseKey);
            if (null != configNode)
            {
                config.Initialize(configNode);
                var systemWebInfo = System.Configuration.ConfigurationManager.GetSection("system.web");
                if (systemWebInfo != null)
                {
                    var systemWeb = (System.Web.Configuration.SystemWebSectionGroup)systemWebInfo;
                    config.Debug = systemWeb.Compilation.Debug;
                }
            }

            appConfig = config;
        }

        /// <summary>
        /// 获取默认线程池
        /// </summary>
        /// <returns></returns>
        public static SmartThreadPool GetThreadPool()
        {
            return ObjectIOCFactory.GetSingleton<SmartThreadPool>();
        }

        /// <summary>
        /// 获取默认任务处理器
        /// </summary>
        /// <returns></returns>
        public static TaskProcessor GetTaskProcessor()
        {
            return ObjectIOCFactory.GetSingleton<TaskProcessor>();
        }

        /// <summary>
        /// 获取配置信息
        /// </summary>
        /// <param name="configSectionName"></param>
        /// <returns></returns>
        public static IConfigNode GetConfigNode(string configSectionName)
        {
            return GetConfigNode(configSectionName, null);
        }

        /// <summary>
        /// 获取配置信息
        /// </summary>
        /// <param name="configSectionName"></param>
        /// <param name="nodePath"></param>
        /// <returns></returns>
        public static IConfigNode GetConfigNode(string configSectionName, string nodePath)
        {
            IConfigNode config = null;
            var configFile = GetConfigPath(configSectionName);
            if (File.Exists(configFile) == false)
            {
                throw new ConfigException(string.Format("cannot find the config : {0}", configFile));
            }

            var xml = new XmlDocument();
            xml.Load(configFile);
            string xpath = null;
            if (string.IsNullOrEmpty(nodePath))
            {
                xpath = string.Format(@"/configuration/{0}", configSectionName);
            }
            else
            {
                xpath = string.Format(@"/configuration/{0}{1}", configSectionName, nodePath);
            }

            var targetNode = xml.SelectSingleNode(xpath);
            if (null != targetNode)
            {
                config = new ConfigNode(targetNode);
            } 
            return config;
        }

        static string GetConfigPath(string configSectionName)
        {
            string configPath = null;

            // read from app.config/sectionName
            var configSection = (XmlConfigSection)ConfigurationManager.GetSection(configSectionName);
            if (configSection != null && configSection.RawXml != null && configSection.RawXml.Length > configSectionName.Length)
            {
                configPath = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None).FilePath;
            }

            // read from app.config/AppSettings[sectionName]            
            if (string.IsNullOrEmpty(configPath))
            {
                var appSettingsKey = string.Concat(configSectionName, ".config");
                configPath = ConfigurationManager.AppSettings[appSettingsKey];
            }

            // read from sectionName.config : 
            if (string.IsNullOrEmpty(configPath))
            {
                var configFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, string.Concat(configSectionName, ".config"));
                if (File.Exists(configFile))
                {
                    configPath = configFile;
                }
            }

            // read from app.config/AppSettings[AppGenome]
            if (string.IsNullOrEmpty(configPath))
            {
                var appSettingsKey = string.Concat(AppConfig.AppGenomeKey, ".config");
                configPath = ConfigurationManager.AppSettings[appSettingsKey];
            }

            // read from AppGenome.config
            if (string.IsNullOrEmpty(configPath))
            {
                var configFile = string.Concat(AppConfig.AppGenomeKey, ".config");
                configFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, configFile);
                if (File.Exists(configFile))
                {
                    configPath = configFile;
                }
            }

            return configPath;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="alias"></param>
        /// <param name="typeDefine"></param>
        public static void RegisterTypeAlias(string alias, string typeDefine)
        {
            AppInstance.Config.RegisterTypeAlias(alias, typeDefine);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static void RegisterTypeAlias<T>()
        {
            var targetType = typeof(T);
            RegisterTypeAlias(targetType.Name, targetType.AssemblyQualifiedName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="alias"></param>
        public static void RegisterTypeAlias<T>(string alias)
        {
            var targetType = typeof(T);
            RegisterTypeAlias(string.Format("{0}.{1}", targetType.Name,alias), targetType.AssemblyQualifiedName);
            //RegisterTypeAlias(alias, targetType.AssemblyQualifiedName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="moduleKey"></param>
        public static void RegisterTypeAliasByModule<T>(string moduleKey)
        {
            var targetType = typeof(T);
            var alias = string.Format("{0}.{1}", moduleKey, targetType.Name);
            RegisterTypeAlias(alias, targetType.AssemblyQualifiedName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="moduleKey"></param>
        /// <param name="module"></param>
        public static void RegisterModule(string moduleKey, string module)
        {
            AppInstance.Config.RegisterModule(moduleKey, module);
        }

        #endregion

        #region Instance Members

        /// <summary>
        /// 
        /// </summary>
        public AppInstance()
        {
            AppInstance.RegisterModules();
            AppInstance.SetDefaultConfig();            
        }

        static void RegisterModules()
        {
            AppInstance.RegisterModule(AppConfig.AppBaseKey, "M2SA.AppGenome");
            AppInstance.RegisterModule(AppConfig.ExceptionHandlingKey, "M2SA.AppGenome.ExceptionHandling");
            AppInstance.RegisterModule(AppConfig.LoggingKey, "M2SA.AppGenome.Logging");
            AppInstance.RegisterModule(AppConfig.QueuesKey, "M2SA.AppGenome.Queues");
            AppInstance.RegisterModule(AppConfig.CachedKey, "M2SA.AppGenome.Cache");
        }

        static void SetDefaultConfig()
        {
            AppInstance.RegisterTypeAlias("IList", "System.Collections.ArrayList");
            AppInstance.RegisterTypeAlias("IList`1", "System.Collections.Generic.List`1");
            AppInstance.RegisterTypeAlias("IDictionary", "System.Collections.Hashtable");
            AppInstance.RegisterTypeAlias("IDictionary`2", "System.Collections.Generic.Dictionary`2");

            AppInstance.RegisterTypeAlias("Exception", typeof(Exception).AssemblyQualifiedName);
            AppInstance.RegisterTypeAlias("HttpException", typeof(System.Web.HttpException).AssemblyQualifiedName);
            AppInstance.RegisterTypeAlias("HttpUnhandledException", typeof(System.Web.HttpUnhandledException).AssemblyQualifiedName);
            AppInstance.RegisterTypeAlias("FatalException", typeof(FatalException).GetSimpleQualifiedName());
            AppInstance.RegisterTypeAlias("TaskThreadException", typeof(TaskThreadException).GetSimpleQualifiedName());
            AppInstance.RegisterTypeAlias("HostileRequestException", typeof(HostileRequestException).GetSimpleQualifiedName());

            AppInstance.RegisterTypeAliasByModule<ExceptionPolicyFactory>(AppConfig.ExceptionHandlingKey);

            //AppInstance.RegisterTypeAlias("LogFactory", "M2SA.AppGenome.Logging.LogRespository,M2SA.AppGenome.Logging");
            //AppInstance.RegisterTypeAlias("CacheFactory", "M2SA.AppGenome.Cache.CacheFactory,M2SA.AppGenome.Cache");
            // AppInstance.RegisterTypeAlias("QueueFactory", "M2SA.AppGenome.MessageQueues.QueueFactory,M2SA.AppGenome.MessageQueues");

            //AppInstance.RegisterTypeAlias(string.Format("{0}.MemCache", AppConfig.CachedKey), "M2SA.AppGenome.Cache.MemCache, M2SA.AppGenome.Cache");
            //AppInstance.RegisterTypeAlias(string.Format("{0}.ActiveCacheNotify", AppConfig.CachedKey), "M2SA.AppGenome.Cache.ActiveCacheNotify, M2SA.AppGenome.Cache");

            //AppInstance.RegisterTypeAlias(string.Format("{0}.MSMQ", AppConfig.QueuesKey), "M2SA.AppGenome.MessageQueues.MSMQ,M2SA.AppGenome.MessageQueues");
            //AppInstance.RegisterTypeAlias(string.Format("{0}.QueueCluster", AppConfig.QueuesKey), "M2SA.AppGenome.MessageQueues.QueueCluster,M2SA.AppGenome.MessageQueues");
            //AppInstance.RegisterTypeAlias(string.Format("{0}.QueueLoadStrategy", AppConfig.QueuesKey), "M2SA.AppGenome.MessageQueues.QueueLoadStrategy,M2SA.AppGenome.MessageQueues");

            AppInstance.RegisterTypeAlias<ExitCommandListener>();

            AppInstance.RegisterTypeAliasByModule<ExceptionPolicy>(AppConfig.ExceptionHandlingKey);
            AppInstance.RegisterTypeAliasByModule<LoggingExceptionHandler>(AppConfig.ExceptionHandlingKey);
            AppInstance.RegisterTypeAliasByModule<WrapHandler>(AppConfig.ExceptionHandlingKey);
            AppInstance.RegisterTypeAliasByModule<ReplaceHandler>(AppConfig.ExceptionHandlingKey);
            AppInstance.RegisterTypeAliasByModule<HttpRedirectHandler>(AppConfig.ExceptionHandlingKey);
        }

        #region IExtensionApplication 成员

        void IExtensionApplication.OnInit(ApplicationHost onwer, CommandArguments args)
        {
            if (AppInstance.Config.Debug)
            {
                var log = LogManager.GetLogger();
                log.Debug("TypeAliases:");
                foreach (var item in AppInstance.Config.TypeAliases)
                {
                    log.Debug("\t {0} -> {1}", item.Key, item.Value);
                }
            }
        }

        void IExtensionApplication.OnStart(ApplicationHost onwer, CommandArguments args)
        {
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
            if (AppInstance.Config.Debug)
            {
                LogManager.GetLogger().Debug("---------- AppInstance.Start... ----------");
            }
        }

        void IExtensionApplication.OnStop(ApplicationHost onwer, CommandArguments args)
        {
            if (AppInstance.Config.Debug)
            {
                LogManager.GetLogger().Debug("---------- AppInstance.Stop... ----------");
            }
        }

        #endregion

        void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var ex = (Exception)e.ExceptionObject;
            new FatalException(ex).HandleException();
        }

        #endregion
    }
}

