using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using M2SA.AppGenome.Configuration;

namespace M2SA.AppGenome.Queues
{
    /// <summary>
    /// 消息接收完时触发的事件委托
    /// </summary>
    /// <param name="message"></param>
    public delegate void ReceiveMessageCompletedEventHandler(object message);

    /// <summary>
    /// 消息队列的接口
    /// </summary>
    public interface IMessageQueue : IDisposable, IResolveObject
    {
        /// <summary>
        /// 队列名称
        /// </summary>
        string Name { get; }

        /// <summary>
        /// 队列路径
        /// </summary>
        string Path { get; }

        /// <summary>
        /// 等待处理的队列个数
        /// </summary>
        long Count { get; }

        /// <summary>
        /// 等待传入的队列个数
        /// </summary>
        long IngoingCount
        {
            get;
        }

        /// <summary>
        /// 
        /// </summary>
        bool CanWrite { get; }

        /// <summary>
        /// 异步读取消息模式，当消息接收完时触发的事件
        /// </summary>
        event ReceiveMessageCompletedEventHandler ReceiveCompleted;

        /// <summary>
        /// 将消息发送到队列
        /// </summary>
        /// <param name="message"></param>
        void Send(object message);

        /// <summary>
        /// 开始异步读取消息，当消息接收完时触发事件ReceiveCompleted
        /// </summary>
        void BeginReceive();

        /// <summary>
        /// 同步读取消息
        /// 当队列没有消息时，阻塞当前线程，等待队列有消息后返回消息体
        /// </summary>
        /// <returns></returns>
        object Receive();
    }
}
