using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using System.Threading;

namespace M2SA.AppGenome.Queues.Tests
{
    public static class TestHelper
    {
        public static void ClearQueue(string queueName)
        {
            var queue = QueueManager.GetQueue(queueName);

            if (queue is QueueCluster)
            {
                var queueCluster = (QueueCluster)queue;
                foreach (var item in queueCluster.WorkQueues)
                {
                    var count = item.Count;
                    for (var i = 0; i < count; i++)
                    {
                        item.Receive();
                    }
                }
            }
            else
            {
                var count = queue.Count;
                for (var i = 0; i < count; i++)
                {
                    queue.Receive();
                }
            }
        }
        public static void SendTest(string queueName, object message)
        {
            var originalCount = QueueManager.GetQueueLength(queueName);

            QueueManager.SendToQueue(message, queueName);
            var actualCount = QueueManager.GetQueueLength(queueName);
            Assert.AreEqual(originalCount + 1, actualCount);
        }

        public static void ReceiveTest(string queueName, object message)
        {
            ClearQueue(queueName);            

            var originalCount = QueueManager.GetQueueLength(queueName);
            Assert.AreEqual(0, originalCount);

            QueueManager.SendToQueue(message, queueName);
            var actualCount = QueueManager.GetQueueLength(queueName);
            Assert.AreEqual(originalCount + 1, actualCount);

            var queue = QueueManager.GetQueue(queueName);
            var receiveMessage = queue.Receive();

            Assert.AreEqual(message, receiveMessage);

            actualCount = QueueManager.GetQueueLength(queueName);
            Assert.AreEqual(originalCount, actualCount);
        }

        public static void BeginReceiveTest(string queueName, object message)
        {
            ClearQueue(queueName);

            var originalCount = QueueManager.GetQueueLength(queueName);
            Assert.AreEqual(0, originalCount);

            var queue = QueueManager.GetQueue(queueName);
            queue.ReceiveCompleted += delegate(object obj)
            {
                var receiveObj = obj;

                Assert.AreEqual(message, receiveObj);

                var actualCount = QueueManager.GetQueueLength(queueName);
                Assert.AreEqual(originalCount, actualCount);

                Console.WriteLine("message Receive:{0}", receiveObj);
            };
            queue.BeginReceive();

            QueueManager.SendToQueue(message, queueName);

            Thread.Sleep(2000);
        }
    }
}
