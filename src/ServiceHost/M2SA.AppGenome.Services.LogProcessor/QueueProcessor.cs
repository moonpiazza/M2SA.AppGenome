using System;
using System.Collections.Generic;
using M2SA.AppGenome.AppHub;
using M2SA.AppGenome.Logging;
using M2SA.AppGenome.Logging.Formatters;
using M2SA.AppGenome.Logging.Listeners;
using M2SA.AppGenome.Queues;
using M2SA.AppGenome.Reflection;

namespace M2SA.AppGenome.Services.LogProcessor
{
    /// <summary>
    /// 
    /// </summary>
    public class QueueProcessor : IExtensionApplication
    {
        static bool IsWriteForListener(LogLevel level, ListenerProxy proxy)
        {
            return (level >= proxy.MinLevel && level <= proxy.MaxLevel);
        }

        private IMessageQueue queue;
        private List<IListener> listeners;

        /// <summary>
        /// 
        /// </summary>
        public string QueueName { get; set; }

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
            this.InitListeners();
            

            if (null == this.queue)
            {
                throw new ArgumentOutOfRangeException("QueueName", string.Format("Not find the queue : {0}", this.QueueName));
            }

            this.queue.ReceiveCompleted += message => queue_ReceiveCompleted(message);
            
            queue.BeginReceive();
        }

        void InitListeners()
        {
            this.listeners = new List<IListener>(2);
            var listenerRespository = new ListenerFactory();
            foreach (var key in this.ListenerIndexes.Keys)
            {
                var proxy = this.ListenerIndexes[key];
                if (proxy.Enabled)
                {
                    var listener = listenerRespository.GetListener(proxy.Name);
                    this.listeners.Add(listener);
                }
            }
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

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        void SaveLogEntry(ILogEntry entry)
        {
            entry.WriteTime = DateTime.Now;
            this.listeners.ForEach(item =>
            {
                if (IsWriteForListener(entry.LogLevel, this.ListenerIndexes[item.Name]))
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
    }
}
