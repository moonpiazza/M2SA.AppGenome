using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Threading;
using M2SA.AppGenome.AppHub;

namespace M2SA.AppGenome.ServiceHost
{
    /// <summary>
    /// 
    /// </summary>
    public class ExternalCommandListener : IExtensionApplication
    {
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
            //empty action            
            this.ApplicationHost = onwer;
            this.CommandArguments = args;
        }

        void IExtensionApplication.OnStart(ApplicationHost onwer, CommandArguments args)
        {

            this.IsRunning = true;
            this.NotifyMessage();
        }

        void IExtensionApplication.OnStop(ApplicationHost onwer, CommandArguments args)
        {
            this.IsRunning = false;
        }

        #endregion

        static void ExcuteCommand(CommandType commandType)
        {
            switch (commandType)
            {
                case CommandType.Restart:
                    {
                        ObjectIOCFactory.GetSingleton<ApplicationContainer>().Restart();
                        break;
                    }
                case CommandType.Exit:
                    {
                        ApplicationHost.GetInstance().Stop();
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
        }

        void NotifyMessage()
        {
            var ct = new Thread(new ThreadStart(NotifyMessageByThread));
            ct.Start();
        }

        void NotifyMessageByThread()
        {
            var restartMessageFile = string.Format("{0}.restart", this.CommandArguments.SessionId);
            restartMessageFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, restartMessageFile);

            while (this.IsRunning)
            {
                Thread.Sleep(1000);

                if (File.Exists(restartMessageFile))
                {
                    this.IsRunning = false;
                    File.Delete(restartMessageFile);
                    Thread.Sleep(1000);

                    ExcuteCommand(CommandType.Restart);
                }
            }
        }
    }
}
