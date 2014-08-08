using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace M2SA.AppGenome.AppHub
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class ExitCommandListener : IExtensionApplication
    {
        /// <summary>
        /// 
        /// </summary>
        public bool AsyncStart { get { return false; } }

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

        void IExtensionApplication.OnInit(ApplicationHost onwer, CommandArguments args)
        {
            ArgumentAssertion.IsNotNull(onwer, "onwer");
            onwer.Exit += new EventHandler(ExtensibleApplication_Exit);
        }

        void ExtensibleApplication_Exit(object sender, EventArgs e)
        {
            System.Environment.Exit(2);
        }

        void IExtensionApplication.OnStart(ApplicationHost onwer, CommandArguments args)
        {
            this.ApplicationHost = onwer;
            this.CommandArguments = args;
            this.IsRunning = true;            
            if (this.CommandArguments.HostProcessId > 0)
            {
                this.NotifyMessage();

                var hostProcess = Process.GetProcessById(this.CommandArguments.HostProcessId);
                hostProcess.EnableRaisingEvents = true;
                hostProcess.Exited += new EventHandler(hostProcess_Exited);

                LogHelper.AppInfo(this.CommandArguments.SessionId, "Started In Host[{0}]", this.CommandArguments.HostProcessId);
            }
        }

        void IExtensionApplication.OnStop(ApplicationHost onwer, CommandArguments args)
        {
            this.IsRunning = false;

            if (this.CommandArguments.HostProcessId > 0)
            {
                LogHelper.AppInfo(this.CommandArguments.SessionId, "Stoped In Host[{0}]", this.CommandArguments.HostProcessId);
            }
        }

        #endregion

        void ExcuteCommand(CommandType commandType)
        {
            switch (commandType)
            {
                case CommandType.Exit:
                    {
                        this.ApplicationHost.Stop();
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
        }

        void hostProcess_Exited(object sender, EventArgs e)
        {
            LogHelper.AppInfo(this.CommandArguments.SessionId, "##########  Exited On MainHost Close[{0}]  ##########", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
            LogHelper.AppInfo(this.CommandArguments.SessionId, "Stop Out Host[{0}:{1}]:{2}", this.CommandArguments.HostProcessId, this.CommandArguments.SessionId, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
            LogHelper.AppInfo(this.CommandArguments.SessionId, "---------------------------------------------------------------------");

            this.ExcuteCommand(CommandType.Exit);
        }

        void NotifyMessage()
        {
            var ct = new Thread(new ThreadStart(NotifyMessageByThread));
            ct.Start();
        }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        void NotifyMessageByThread()
        {
            var exitMessageFile = string.Format("{0}.exit", this.CommandArguments.SessionId);
            exitMessageFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, exitMessageFile);

            while (this.IsRunning)
            {
                Thread.Sleep(1000);

                if (File.Exists(exitMessageFile))
                {
                    this.IsRunning = false;
                    for (var i = 0; i < 3; i++)
                    {
                        try
                        {
                            File.Delete(exitMessageFile);
                            break;
                        }
                        catch
                        {
                            Thread.Sleep(1000);
                        }
                    }

                    LogHelper.AppInfo(this.CommandArguments.SessionId, "Stop In Host[{0}:{1}]:{2}", this.CommandArguments.HostProcessId, this.CommandArguments.SessionId, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                    LogHelper.AppInfo(this.CommandArguments.SessionId, "---------------------------------------------------------------------");

                    this.ExcuteCommand(CommandType.Exit);
                }
            }
        }
    }
}
