using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using M2SA.AppGenome.Configuration;

namespace M2SA.AppGenome
{
    /// <summary>
    /// 
    /// </summary>
    public class AppConfig : IResolveObject
    {
        static readonly object syncObject = new object();

        /// <summary>
        /// 
        /// </summary>
        public static readonly string AppGenomeKey = "appgenome";

        /// <summary>
        /// 
        /// </summary>
        public static readonly string AppBaseKey = "appbase";

        /// <summary>
        /// 
        /// </summary>
        public static readonly string ObjectIOCKey = "objectIOC";

        /// <summary>
        /// 
        /// </summary>
        public static readonly string AppHostKey = "apphost";

        /// <summary>
        /// 
        /// </summary>
        public static readonly string LoggingKey = "logging";

        /// <summary>
        /// 
        /// </summary>
        public static readonly string CachedKey = "cached";

        /// <summary>
        /// 
        /// </summary>
        public static readonly string QueuesKey = "queues";

        /// <summary>
        /// 
        /// </summary>
        public static readonly string ExceptionHandlingKey = "exceptionHandling";

        /// <summary>
        /// 
        /// </summary>
        public static readonly string DefaultKey = "default";

        /// <summary>
        /// 配置节点为Map类型，默认的Key节点名称
        /// </summary>
        public static readonly string[] SrongNameSequence = new string[] { "key", "name" }; 

        /// <summary>
        /// 
        /// </summary>
        public string AppName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool Debug { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public IDictionary<string, string> Modules
        {
            get;
            private set;
        }

        /// <summary>
        /// 
        /// </summary>
        public IDictionary<string, string> TypeAliases
        {
            get;
            private set;
        }

        /// <summary>
        /// 
        /// </summary>
        public ThreadPoolConfig ThreadPool
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public AppConfig()
        {
            this.TypeAliases = new Dictionary<string, string>(16);
            this.Modules = new Dictionary<string, string>(8);
            this.ThreadPool = new ThreadPoolConfig();
        }

        #region IResolveObject Members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="config"></param>
        public void Initialize(IConfigNode config)
        {
            this.DeserializeObject(config);
        }

        #endregion

        #region TypeAlias

        /// <summary>
        /// 
        /// </summary>
        /// <param name="alias"></param>
        /// <param name="typeDefine"></param>
        internal void RegisterTypeAlias(string alias, string typeDefine)
        {
            lock (syncObject)
            {
                this.TypeAliases[alias] = typeDefine;                
            }
        }

        #endregion

        #region Modules

        /// <summary>
        /// 
        /// </summary>
        /// <param name="moduleKey"></param>
        /// <param name="module"></param>
        internal void RegisterModule(string moduleKey, string module)
        {
            lock (syncObject)
            {
                this.Modules[moduleKey] = module;
            }
        }

        #endregion

    }
}
