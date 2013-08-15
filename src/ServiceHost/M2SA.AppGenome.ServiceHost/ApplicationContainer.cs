using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.IO;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Reflection;
using M2SA.AppGenome.AppHub;

namespace M2SA.AppGenome.ServiceHost
{
    /// <summary>
    /// 
    /// </summary>
    public class ApplicationContainer : IExtensionApplication
    {
        IList<IExtensionApplication> extensions;
        FileProcessor fileManager = null;

        /// <summary>
        /// 
        /// </summary>
        public ApplicationHost ApplicationHost
        {
            get;
            private set;
        }

        /// <summary>
        /// 
        /// </summary>
        public CommandArguments CommandArguments
        {
            get;
            private set;
        }

        /// <summary>
        /// 
        /// </summary>
        public string MainAppId
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public Process MainAppHandle
        {
            get;
            private set;
        }

        /// <summary>
        /// 
        /// </summary>
        public bool DisabledApp
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public ApplicationContainer()
        {
            this.fileManager = new FileProcessor();
            this.extensions = new List<IExtensionApplication>();
            this.extensions.Add(new ExternalCommandListener());
            this.extensions.Add(this.fileManager);
        }

        #region IExtensionApplication 成员

        /// <summary>
        /// 
        /// </summary>
        /// <param name="onwer"></param>
        /// <param name="args"></param>
        public void OnInit(ApplicationHost onwer, CommandArguments args)
        {
            this.ApplicationHost = onwer;
            this.CommandArguments = args;
            this.MainAppId = Guid.NewGuid().ToString();            

            var currentProcess = Process.GetCurrentProcess();
            this.ApplicationHost.HostProcessId = currentProcess.Id;
            this.ApplicationHost.HostProcessName = currentProcess.ProcessName;
            this.CommandArguments.SessionId = this.MainAppId;

            foreach (var item in this.extensions)
            {
                item.OnInit(this.ApplicationHost, this.CommandArguments);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="onwer"></param>
        /// <param name="args"></param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:丢失范围之前释放对象")]
        public void OnStart(ApplicationHost onwer, CommandArguments args)
        {
            foreach (var item in this.extensions)
            {
                item.OnStart(this.ApplicationHost, this.CommandArguments);
            }

            var hostConfig = ObjectIOCFactory.GetSingleton<ServiceHostConfig>();
            var targetAppName = this.fileManager.GetRunAppFileName(hostConfig.App.ExeFile);
            var targetAppPath = Path.Combine(hostConfig.Host.RunDirectory, targetAppName);
             this.DisabledApp = false;

            var mainProcess = new Process();
            mainProcess.EnableRaisingEvents = true;
            mainProcess.Exited += new EventHandler(mainApp_Exited);
            mainProcess.StartInfo.FileName = targetAppPath;
            mainProcess.StartInfo.Arguments = this.GetAppArguments();

            var enableDebug = false;
            if (hostConfig.Host.RunInService == false && hostConfig.App.Debug == true)
                enableDebug = true;

            if (enableDebug)
            {
                mainProcess.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
                mainProcess.StartInfo.CreateNoWindow = false;
            }
            else
            {
                mainProcess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                mainProcess.StartInfo.CreateNoWindow = true;
            }

            Console.WriteLine("[{0}]{1}", mainProcess.StartInfo.FileName, mainProcess.StartInfo.Arguments);

            try
            {
                mainProcess.Start();
            }
            catch (Exception ex)
            {
                new FatalException(string.Concat(ex.Message, " - ", targetAppPath, " - ", mainProcess.StartInfo.Arguments), ex)
                .HandleException();

                ApplicationHost.GetInstance(null).Stop();
            }     
            
            if (null != this.MainAppHandle)
            {
                this.MainAppHandle.Refresh();
            }
            this.MainAppHandle = mainProcess;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="onwer"></param>
        /// <param name="args"></param>
        public void OnStop(ApplicationHost onwer, CommandArguments args)
        {
            this.WaitForMainAppExit();
            foreach (var item in this.extensions)
            {
                item.OnStop(this.ApplicationHost, this.CommandArguments);
            }
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        public void Restart()
        {
            this.OnStop(this.ApplicationHost, this.CommandArguments);
            Thread.Sleep(1500);
            this.OnStart(this.ApplicationHost, this.CommandArguments);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        void WaitForMainAppExit()
        {
            var exitMessageFile = string.Format("{0}.exit", this.MainAppId);
            exitMessageFile = Path.Combine(ObjectIOCFactory.GetSingleton<ServiceHostConfig>().Host.RunDirectory, exitMessageFile);

            if (File.Exists(exitMessageFile) == false)
            {
                var nowText = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");

                for (var i = 0; i < 3; i++)
                {
                    try
                    {
                        using (var sw = new StreamWriter(exitMessageFile))
                        {
                            sw.WriteLine(nowText);
                        }
                        break;
                    }
                    catch
                    {
                        Thread.Sleep(1000);
                    }
                }

                Console.WriteLine("WaitForMainAppExit:{0}[{1}]", nowText, exitMessageFile);
            }
            this.DisabledApp = true;
            this.MainAppHandle.WaitForExit();
        }

        void mainApp_Exited(object sender, EventArgs e)
        {
            Console.WriteLine("mainApp_Exited:" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
            if (this.DisabledApp == false)
            {
                LogHelper.HostInfo(this.MainAppId, "##########  Exited On MainApp Close[{0}]  ##########", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                Process.GetCurrentProcess().Kill();
            }
        }

        string GetAppArguments()
        {
            var result = string.Empty;
            result = this.CommandArguments.ToString();
            result += string.Format(" -appId:{0} -hostPID:{1}", this.MainAppId, this.ApplicationHost.HostProcessId);
            return result;
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var log = string.Format("{0}_UnhandledExceptio.log", DateTime.Now.ToString("yyyyMMddHH"));
            var logFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, log);
            if (e.ExceptionObject != null)
            {
                var ex = e.ExceptionObject as Exception;
                using (var ws = File.AppendText(logFile))
                {
                    ws.WriteLine("===========================================================");
                    ws.WriteLine("Ex:{0}", ex.Message);
                    ws.WriteLine("StackTrace:{0}", ex.StackTrace);
                    ws.WriteLine();
                }
            }
        }
    }
}
