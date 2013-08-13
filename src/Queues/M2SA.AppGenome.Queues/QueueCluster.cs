using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Xml;
using System.Messaging;
using M2SA.AppGenome.Configuration;

namespace M2SA.AppGenome.Queues
{
    /// <summary>
    /// 
    /// </summary>
    public class QueueCluster : ResolveObjectBase, IMessageQueue
    {
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
        public IQueueLoadStrategy LoadStrategy
        {
            get;
            private set;
        }

        /// <summary>
        /// 
        /// </summary>
        public IList<IMessageQueue> WorkQueues
        {
            get;
            private set;
        }

        /// <summary>
        /// 
        /// </summary>
        public QueueCluster()
        {
            this.Timeout = MessageQueue.InfiniteTimeout;
            this.Priority = MessagePriority.Normal;
        }

        #region IResolveObject Members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="config"></param>
        public override void Initialize(IConfigNode config)
        {
            this.WorkQueues = new List<IMessageQueue>(2);
            base.Initialize(config);

            if (this.LoadStrategy == null)
                throw new ConfigException(string.Format("not find LoadStrategy for Queues[{0}]", this.Name));

            this.LoadStrategy.Initialize(this.Name, this.WorkQueues);
        }

        #endregion

        #region IMessageQueue Members

        /// <summary>
        /// 
        /// </summary>
        public event ReceiveMessageCompletedEventHandler ReceiveCompleted
        {
            add
            {
                foreach (var queue in this.WorkQueues)
                {
                    queue.ReceiveCompleted += value;
                }
            }
            remove
            {
                foreach (var queue in this.WorkQueues)
                {
                    queue.ReceiveCompleted -= value;
                }
            }
        }

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
            get
            {
                return string.Format("/QueueCluster/{0}", this.Name);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public long Count
        {
            get
            {
                var count = 0L;
                foreach (var queue in this.WorkQueues)
                {
                    count += queue.Count;
                }
                return count;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public long IngoingCount
        {
            get
            {
                var count = 0L;
                foreach (var queue in this.WorkQueues)
                {
                    count += queue.IngoingCount;
                }
                return count;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool CanWrite
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public void Send(object message)
        {
            var targetQueue = this.LoadStrategy.GetSendQueue();
            targetQueue.Send(message);
        }

        /// <summary>
        /// 
        /// </summary>
        public void BeginReceive()
        {
            var targetQueue = this.LoadStrategy.GetReceiveQueue();
            targetQueue.BeginReceive();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public object Receive()
        {
            var targetQueue = this.LoadStrategy.GetReceiveQueue();
            return targetQueue.Receive();
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
            if (true == disposing && null != this.WorkQueues)
            {
                for (var i = 0; i < this.WorkQueues.Count; i++)
                {
                    this.WorkQueues[i].Dispose();
                }
            }
        }

        #endregion
    }
}
