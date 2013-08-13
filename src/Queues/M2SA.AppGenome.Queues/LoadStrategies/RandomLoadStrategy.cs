using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using M2SA.AppGenome.Logging;

namespace M2SA.AppGenome.Queues.LoadStrategies
{
    /// <summary>
    /// 
    /// </summary>
    public class RandomLoadStrategy : IQueueLoadStrategy
    {
        private IList<IMessageQueue> workQueues;
        private string clusterName;

        #region IQueueLoadStrategy 成员

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="queues"></param>
        public void Initialize(string name, IList<IMessageQueue> queues)
        {
            this.clusterName = name;
            this.workQueues = queues;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IMessageQueue GetSendQueue()
        {
            var index = new Random().Next(0, this.workQueues.Count);
            var targetQueue = this.workQueues[index];
            return targetQueue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IMessageQueue GetReceiveQueue()
        {
            var targetQueues = new List<IMessageQueue>(this.workQueues.Count);
            foreach (var queueItem in this.workQueues)
            {
                if (queueItem.Count > 0)
                {
                    targetQueues.Add(queueItem);
                }
            }
            if (targetQueues.Count == 0)
                targetQueues.AddRange(workQueues);

            var index = new Random().Next(0, targetQueues.Count);
            var targetQueue = targetQueues[index];

            LogManager.GetLogger(QueueFactory.QueueLogger).Trace("ReceiveQueue[{0}] : {1}", this.clusterName, targetQueue.Path);
            return targetQueue;
        }

        #endregion
    }

}
