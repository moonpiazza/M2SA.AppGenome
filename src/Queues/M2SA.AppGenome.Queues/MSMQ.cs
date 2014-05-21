using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Xml;
using System.Messaging;
using M2SA.AppGenome.Configuration;
using M2SA.AppGenome.Reflection;
using M2SA.AppGenome.Logging;
using M2SA.AppGenome.Diagnostics;

namespace M2SA.AppGenome.Queues
{
    /// <summary>
    /// MSMQ消息队列的实现封装
    /// </summary>
    public class MSMQ : ResolveObjectBase, IMessageQueue
    {
        /// <summary>
        /// 获取指定的MSMQ的队列长度
        /// </summary>
        /// <param name="queuePath"></param>
        /// <param name="machineName"></param>
        /// <returns></returns>
        public static long GetQueueLength(string queuePath, string machineName)
        {
            if (null == queuePath)
                throw new ArgumentNullException("queuePath");
            if (null == machineName)
                throw new ArgumentNullException("machineName");

            var count = 0L;
            var queueInfo = queuePath.ToLower();
            if (queueInfo.StartsWith("FormatName:DIRECT=".ToLower()))
            {
                queueInfo = queueInfo.Substring("FormatName:DIRECT=".Length);
            }
            else if (queueInfo.StartsWith("."))
            {
                queueInfo = queueInfo.Substring(2);
            }

            var counterCategory = PerfmonCounterManager.GetCounterCategory("MSMQ Queue", machineName);
            var existEqualsInstanceName = false;
            foreach (var counterInstanceName in counterCategory.Instances.Keys)
            {
                if (counterInstanceName.ToLower() == queueInfo)
                {
                    existEqualsInstanceName = true;
                    count = PerfmonCounterManager.GetCounterItemValue("Messages in Queue", counterInstanceName, "MSMQ Queue", machineName);
                }
            }

            if (existEqualsInstanceName == false)
            {
                foreach (var counterInstanceName in counterCategory.Instances.Keys)
                {
                    if (counterInstanceName.ToLower().EndsWith(queueInfo))
                    {
                        count = PerfmonCounterManager.GetCounterItemValue("Messages in Queue", counterInstanceName, "MSMQ Queue", machineName);
                    }
                }
            }
            return count;
        }

        private static readonly object syncRoot = new object();

        #region Instance Fields

        private MessageQueueTransactionType transactionType = MessageQueueTransactionType.Single;
        private MessageQueue queue;

        /// <summary>
        /// 
        /// </summary>
        public event ReceiveMessageCompletedEventHandler ReceiveCompleted;

        #endregion

        /// <summary>
        /// 
        /// </summary>
        public TimeSpan Timeout
        {
            get;
            private set;
        }

        /// <summary>
        /// 
        /// </summary>
        public MessagePriority Priority
        {
            get;
            private set;
        }

        /// <summary>
        /// 
        /// </summary>
        public IMessageFormatter Formatter
        {
            get;
            private set;
        }

        /// <summary>
        /// 
        /// </summary>
        public MSMQ()
        {
            this.Timeout = MessageQueue.InfiniteTimeout;
            this.Priority = MessagePriority.Normal;
        }        

        void queue_ReceiveCompleted(object sender, ReceiveCompletedEventArgs e)
        {
            var obj = e.Message.Body;
            this.ReceiveCompleted(obj);
        }

        #region IResolveObject Members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="config"></param>
        public override void Initialize(IConfigNode config)
        {
            base.Initialize(config);
            this.queue = new MessageQueue(this.Path);
            this.queue.DefaultPropertiesToSend.Priority = this.Priority;

            // Performance optimization since we don't need these features
            this.queue.DefaultPropertiesToSend.AttachSenderId = false;
            this.queue.DefaultPropertiesToSend.UseAuthentication = false;
            this.queue.DefaultPropertiesToSend.UseEncryption = false;
            this.queue.DefaultPropertiesToSend.AcknowledgeType = AcknowledgeTypes.None;
            this.queue.DefaultPropertiesToSend.UseJournalQueue = false;
            this.queue.DefaultPropertiesToSend.Recoverable = true;
            if (this.Formatter == null)
            {
                this.Formatter = new BinaryMessageFormatter();
            }
            this.queue.Formatter = this.Formatter;
            this.queue.ReceiveCompleted += new ReceiveCompletedEventHandler(queue_ReceiveCompleted);
        }

        #endregion

        #region IMessageQueue Members

        /// <summary>
        /// 
        /// </summary>
        public string Name
        {
            get;
            private set;
        }

        /// <summary>
        /// 
        /// </summary>
        public string Path
        {
            get;
            private set;
        }

        /// <summary>
        /// 
        /// </summary>
        public long Count
        {
            get
            {
                return GetQueueLength(this.queue.Path, this.queue.MachineName);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public long IngoingCount
        {
            get
            {
                return GetQueueLength(this.queue.Path, ".");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool CanWrite
        {
            get
            {
                return this.queue.CanWrite;
            }
        }        

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public void Send(object message)
        {
            if (null == message)
                throw new ArgumentNullException("message");

            lock (syncRoot)
            {
                this.queue.Send(message, transactionType);
            }            
        }

        /// <summary>
        /// 
        /// </summary>
        public void BeginReceive()
        {
            this.queue.BeginReceive(this.Timeout);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public object Receive()
        {
            try
            {
                using (Message message = this.queue.Receive(this.Timeout, transactionType))
                {
                    var entity = message.Body;
                    return entity;
                }
            }
            catch (MessageQueueException mqex)
            {
                if (mqex.MessageQueueErrorCode == MessageQueueErrorCode.IOTimeout)
                    throw new TimeoutException();

                throw;
            }
        }

        #endregion

        #region IDisposable Members

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        private void Dispose(bool disposing)
        {
            if (true == disposing && null != this.queue)
            {
                this.queue.Dispose();
                this.queue = null;
            }
        }
        #endregion        
    }
}
