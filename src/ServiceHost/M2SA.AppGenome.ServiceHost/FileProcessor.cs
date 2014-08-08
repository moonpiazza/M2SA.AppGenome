using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using M2SA.AppGenome.AppHub;
using M2SA.AppGenome.Logging;

namespace M2SA.AppGenome.ServiceHost
{
    /// <summary>
    /// 
    /// </summary>
    public class FileProcessor : IExtensionApplication
    {
        public bool AsyncStart { get { return false; } }

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
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="onwer"></param>
        /// <param name="args"></param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        public void OnStart(ApplicationHost onwer, CommandArguments args)
        {
            try
            {
                var syncSuccessFlag = false;
                while (syncSuccessFlag == false)
                {
                    try
                    {
                        var hostConfig = ObjectIOCFactory.GetSingleton<ServiceHostConfig>();
                        var source = hostConfig.Host.SourceDirectory;
                        var target = hostConfig.Host.RunDirectory;
                        if (Directory.Exists(target) == false)
                        {
                            Directory.CreateDirectory(target);
                        }

                        var xcopyArgs = string.Format("{0}\\* {1} /s /y /d", source, target);

                        using (var copyProcess = new Process())
                        {
                            copyProcess.EnableRaisingEvents = true;
                            copyProcess.StartInfo.FileName = "xcopy";
                            copyProcess.StartInfo.Arguments = xcopyArgs;
                            copyProcess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                            copyProcess.Start();
                            copyProcess.WaitForExit();
                        }

                        this.DeleteTempAppFiles();
                        this.RenameAppFiles();

                        syncSuccessFlag = true;
                    }
                    catch (Exception ex)
                    {
                        LogManager.GetLogger().Error(ex);
                        Console.WriteLine("{0}:{1}", ex.GetType().FullName, ex.Message);
                        Console.WriteLine("StackTrace:{0}", ex.StackTrace);
                        Thread.Sleep(1000);
                    }
                }
            }
            catch (Exception ex)
            {
                LogManager.GetLogger().Error(ex);
                Console.WriteLine("Ex:{0}", ex.Message);
                Console.WriteLine("StackTrace:{0}", ex.StackTrace);
                Console.ReadLine();
                return;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="onwer"></param>
        /// <param name="args"></param>
        public void OnStop(ApplicationHost onwer, CommandArguments args)
        {
            //empty action
        }

        #endregion
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sourceFileName"></param>
        /// <returns></returns>
        public string GetRunAppFileName(string sourceFileName)
        {
            if (ObjectIOCFactory.GetSingleton<ServiceHostConfig>().App.RenameForHost == false)
                return sourceFileName;
            return string.Format("{0}[{1}]_{2}", this.ApplicationHost.HostProcessName, this.ApplicationHost.HostProcessId, sourceFileName);
        }
        void DeleteTempAppFiles()
        {
            var hostConfig = ObjectIOCFactory.GetSingleton<ServiceHostConfig>();
            if (Directory.Exists(hostConfig.Host.RunDirectory))
            {
                var searchPattern = string.Format("{0}[*]_{1}.*", this.ApplicationHost.HostProcessName, Path.GetFileNameWithoutExtension(hostConfig.App.ExeFile));
                var appFiles = Directory.GetFiles(hostConfig.Host.RunDirectory, searchPattern);
                foreach (var appFile in appFiles)
                {
                    File.Delete(appFile);
                }
            }
        }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2201:DoNotRaiseReservedExceptionTypes")]
        void RenameAppFiles()
        {
            var hostConfig = ObjectIOCFactory.GetSingleton<ServiceHostConfig>();
            var mainAppPath = Path.Combine(hostConfig.Host.RunDirectory, hostConfig.App.ExeFile);
            if (File.Exists(mainAppPath))
            {
                var searchPattern = string.Format("{0}.*", Path.GetFileNameWithoutExtension(hostConfig.App.ExeFile));
                var appFiles = Directory.GetFiles(hostConfig.Host.RunDirectory, searchPattern);
                foreach (var appFile in appFiles)
                {
                    RenameAppFile(Path.GetFileName(appFile));
                }
            }
            else
            {
                throw new Exception(string.Format("没有找到文件：{0}", mainAppPath));
            }
        }
        void RenameAppFile(string sourceFileName)
        {
            var hostConfig = ObjectIOCFactory.GetSingleton<ServiceHostConfig>();
            if (hostConfig.App.RenameForHost == false)
                return;
            var sourceFilePath = Path.Combine(hostConfig.Host.RunDirectory, sourceFileName);
            var targetFileName = this.GetRunAppFileName(sourceFileName);
            var targetFilePath = Path.Combine(hostConfig.Host.RunDirectory, targetFileName);
            if (File.Exists(targetFilePath))
            {
                File.Delete(targetFilePath);
            }
            File.Move(sourceFilePath, targetFilePath);
        }
    }
}