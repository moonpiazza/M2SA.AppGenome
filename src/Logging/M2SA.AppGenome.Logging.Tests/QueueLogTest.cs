using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.IO;
using M2SA.AppGenome.Queues;
using NUnit.Framework;

namespace M2SA.AppGenome.Logging.Tests
{
    /// <summary>
    /// 
    /// </summary>
    /// <include file='appgenome.config' />
    [TestFixture]
    public class QueueLogTest : TestBase
    {
        [Test]
        public void TestLogInfo()
        {
            var queueName = "TestAppLog";
            var queueLog = "QueueTest";
            ClearQueue(queueName);
            var originalCount = QueueManager.GetQueueLength(queueName);
            Assert.AreEqual(0, originalCount);

            var message = string.Format("[queue]{0}", TestHelper.RandomizeString());
            LogManager.GetLogger(queueLog).Info(message);

            var queue = QueueManager.GetQueue(queueName);
            var receiveMessage = queue.Receive();
            Assert.IsTrue(receiveMessage is LogEntry);

            var entry = (LogEntry)receiveMessage;
            Assert.AreEqual(message, entry.Message);
        }

        static void ClearQueue(string queueName)
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
    }
}
