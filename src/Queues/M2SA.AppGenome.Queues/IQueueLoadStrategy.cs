using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace M2SA.AppGenome.Queues
{
    /// <summary>
    /// 
    /// </summary>
    public interface IQueueLoadStrategy
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="queues"></param>
        void Initialize(string name, IList<IMessageQueue> queues);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IMessageQueue GetSendQueue();
        
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IMessageQueue GetReceiveQueue();
    }
}
