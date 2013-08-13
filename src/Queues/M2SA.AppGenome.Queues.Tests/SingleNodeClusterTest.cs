using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using M2SA.AppGenome.Queues.Tests.TestObjects;

namespace M2SA.AppGenome.Queues.Tests
{
    [TestFixture]
    public class SingleNodeClusterTest : TestBase
    {
        private string queueName = "testClusterB";

        [Test]
        public void SendTest()
        {
            var message = new TestMessage()
            {
                Name = new Random().Next(10000).ToString()
            };

            TestHelper.SendTest(queueName, message);
        }

        [Test]
        public void ReceiveTest()
        {
            var message = new TestMessage()
            {
                Name = new Random().Next(10000).ToString()
            };
            TestHelper.ReceiveTest(queueName, message);
        }

        [Test]
        public void BeginReceiveTest()
        {
            var message = new TestMessage()
            {
                Name = new Random().Next(10000).ToString()
            };
            TestHelper.BeginReceiveTest(queueName, message);
        }
    }
}

