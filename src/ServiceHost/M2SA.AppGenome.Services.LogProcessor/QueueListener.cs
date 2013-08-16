using System;
using System.Collections.Generic;
using System.Threading;
using System.Text;
using System.IO;
using M2SA.AppGenome.AppHub;
using M2SA.AppGenome.Cache;
using M2SA.AppGenome.Logging;
using M2SA.AppGenome.Logging.Formatters;
using M2SA.AppGenome.Queues;
using M2SA.AppGenome.Reflection;
using M2SA.AppGenome.Threading;

namespace M2SA.AppGenome.Services.LogProcessor
{
    /// <summary>
    /// 
    /// </summary>
    public class QueueListener : IExtensionApplication
    {
        private IMessageQueue queue;

        /// <summary>
        /// 
        /// </summary>
        public string QueueName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsRunning { get; private set; }

        #region IExtensionApplication 成员

        void IExtensionApplication.OnInit(ApplicationHost onwer, CommandArguments args)
        {
            //empty action
        }

        void IExtensionApplication.OnStart(ApplicationHost onwer, CommandArguments args)
        {
            this.IsRunning = true;
            Console.WriteLine("---------- Processor.OnStart... ----------");

            Process();
        }

        void IExtensionApplication.OnStop(ApplicationHost onwer, CommandArguments args)
        {
            Console.WriteLine("---------- Processor.OnStop... ----------");
            this.IsRunning = false;
        }

        #endregion


        void Process()
        {
            this.queue = QueueManager.GetQueue(this.QueueName);

            if (null == this.queue)
            {
                throw new ArgumentOutOfRangeException("QueueName", string.Format("Not find the queue : {0}", this.QueueName));
            }

            this.queue.ReceiveCompleted += message => queue_ReceiveCompleted(message);
            
            queue.BeginReceive();
        }

        void queue_ReceiveCompleted(object message)
        {
            AppInstance.GetThreadPool().QueueWorkItem(() => 
            { 
                if (false == (message is ILogEntry))
                {
                    var filePath = string.Format("logs\\{0}-{1}.log", message.GetType().Name, DateTime.Now.ToString("yyyy-MM-dd-hh"));
                    var content = message.ToText();
                    Console.WriteLine(content);
                    FileHelper.WriteInfo(filePath, content);
                }
                else
                {
                    SaveLogEntry(message as ILogEntry);
                }
            });
            queue.BeginReceive();
        }

        static void SaveLogEntry(ILogEntry entry)
        {
            var filePath = string.Format("logs\\{0}.log", entry.LogTime.ToString("yyyy-MM-dd-hh"));

            var content = new TextFormatter().Format(entry).ToString();

            Console.WriteLine(content);
            FileHelper.WriteInfo(filePath, content);
        }
    }
}
