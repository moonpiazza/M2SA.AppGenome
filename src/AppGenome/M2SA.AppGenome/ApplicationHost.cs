using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using M2SA.AppGenome.AppHub;
using M2SA.AppGenome.Logging;
using M2SA.AppGenome.Threading;
using M2SA.AppGenome.Reflection;

namespace M2SA.AppGenome
{
    /// <summary>
    /// 
    /// </summary>
    public class ApplicationHost
    {
        static readonly object SyncRoot = new object();

        static ApplicationHost Instance = null;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static ApplicationHost GetInstance(params string[] args)
        {
            ApplicationHost result = Instance;
            if (null == Instance)
            {
                lock (SyncRoot)
                {
                    if (null != Instance)
                    {
                        result = Instance;
                    }
                    else
                    {
                        Instance = new ApplicationHost(args);
                        result = Instance;
                    }
                }                
            }
            return result;
        }

        bool isInit = false;

        IList<IExtensionApplication> extensions;

        /// <summary>
        /// 
        /// </summary>
        public event EventHandler Exit;

        /// <summary>
        /// 
        /// </summary>
        public bool IsRunning
        {
            get;
            private set;
        }

        /// <summary>
        /// 
        /// </summary>
        public int HostProcessId
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public string HostProcessName
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public CommandArguments CommandArguments
        {
            get;
            private set;
        }

        private ApplicationHost(params string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
            AppDomain.CurrentDomain.DomainUnload += new EventHandler(CurrentDomain_DomainUnload);
            AppDomain.CurrentDomain.ProcessExit += new EventHandler(CurrentDomain_ProcessExit);

            this.CommandArguments = new CommandArguments(args);
            this.extensions = new List<IExtensionApplication>()
            {
                ObjectIOCFactory.GetSingleton<AppInstance>(),
                AppInstance.GetTaskProcessor(),
                ObjectIOCFactory.GetSingleton<ApplicationHub>(),
            };
        }

        void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            if (AppInstance.Config.Debug)            
                LogManager.GetLogger().Info("Stop By CurrentDomain_ProcessExit");
            this.Stop();
        }

        void CurrentDomain_DomainUnload(object sender, EventArgs e)
        {
            if (AppInstance.Config.Debug)
                LogManager.GetLogger().Info("Stop By CurrentDomain_DomainUnload");
            this.Stop();
        }

        void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject != null)
            {
                var log = string.Format("{0}_UnhandledException.log", DateTime.Now.ToString("yyyyMMddHH"));
                FileHelper.WriteInfo(log, e.ExceptionObject.ToText());
            }

            if (AppInstance.Config.Debug)
                LogManager.GetLogger().Info("Stop By CurrentDomain_UnhandledException");

            var exSource = (Exception)e.ExceptionObject;
            new FatalException(exSource).HandleException();
            this.Stop();
        }

        /// <summary>
        /// 
        /// </summary>
        public void Init()
        {
            for (var i = 0; i < this.extensions.Count; i++)
            {
                this.extensions[i].OnInit(this, this.CommandArguments);
            }
            this.isInit = true;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Start()
        {
            if (this.IsRunning)
            {
                return;
            }

            lock (SyncRoot)
            {
                if (this.IsRunning)
                {
                    return;
                }

                if (this.isInit == false)
                {
                    this.Init();
                }

                for (var i = 0; i < this.extensions.Count; i++)
                {
                    var extension = this.extensions[i];
                    if (false == extension.AsyncStart)
                        extension.OnStart(this, this.CommandArguments);
                }
                for (var i = 0; i < this.extensions.Count; i++)
                {
                    var extension = this.extensions[i];
                    if (true == this.extensions[i].AsyncStart)
                        new Thread(() => extension.OnStart(this, this.CommandArguments)).Start();
                }

                this.IsRunning = true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        public void Stop()
        {
            if (this.IsRunning == false)
            {
                return;
            }

            lock (SyncRoot)
            {
                if (this.IsRunning == false)
                {
                    return;
                }

                for (var i = this.extensions.Count - 1; i >= 0; i--)
                {
                    try
                    {
                        this.extensions[i].OnStop(this, this.CommandArguments);
                    }
                    catch (Exception ex)
                    {
                        new FatalException(ex).HandleException();
                    }
                }
                this.IsRunning = false;
                this.OnExit();
            }
        }

        void OnExit()
        {
            if (this.Exit != null)
            {
                this.Exit(this, new EventArgs());
            }
        } 
    }
}
