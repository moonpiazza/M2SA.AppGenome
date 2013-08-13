using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using M2SA.AppGenome.Diagnostics;

namespace M2SA.AppGenome.Queues
{
    /// <summary>
    /// 
    /// </summary>
    public static class QueueManager
    {
        /// <summary>
        /// 获取IMessageQueue的默认实例
        /// </summary>
        /// <param name="queueName"></param>
        /// <returns></returns>
        public static  IMessageQueue GetQueue(string queueName)
        {
            return ObjectIOCFactory.GetSingleton<IQueueFactory>().GetQueue(queueName);
        }

        /// <summary>
        /// 根据IMessageQueue关联的类型获取IMessageQueue实例
        /// </summary>
        /// <param name="dataType"></param>
        /// <returns></returns>
        public static IMessageQueue GetQueueByDataType(string dataType)
        {
            return ObjectIOCFactory.GetSingleton<IQueueFactory>().GetQueue(dataType);
        }

        /// <summary>
        /// 将消息发送到指定队列
        /// </summary>
        /// <param name="message"></param>
        /// <param name="queueName"></param>
        public static void SendToQueue(object message, string queueName)
        {
            var queue = GetQueue(queueName);
            queue.Send(message);
        }

        /// <summary>
        /// 获取指定队列的队列长度
        /// </summary>
        /// <param name="queueName"></param>
        /// <returns></returns>
        public static long GetQueueLength(string queueName)
        {
            var queue = GetQueue(queueName);
            return queue.Count;
        }
    }
}
