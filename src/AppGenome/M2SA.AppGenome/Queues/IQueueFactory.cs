using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace M2SA.AppGenome.Queues
{
    /// <summary>
    /// 
    /// </summary>
    public interface IQueueFactory
    {
        /// <summary>
        /// 获取IMessageQueue的默认实例
        /// </summary>
        /// <param name="queueName"></param>
        /// <returns></returns>
        IMessageQueue GetQueue(string queueName);

        /// <summary>
        /// 获取IMessageQueue的指定类别的实例
        /// </summary>
        /// <param name="dataType"></param>
        /// <returns></returns>
        IMessageQueue GetQueueByDataType(string dataType);

        /// <summary>
        /// 是否存在指定类别的IMessageQueue
        /// </summary>
        /// <param name="queueName"></param>
        /// <returns></returns>
        bool ExistsQueue(string queueName);
    }
}
