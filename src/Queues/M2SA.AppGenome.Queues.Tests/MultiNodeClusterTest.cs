using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using M2SA.AppGenome.Queues.Tests.TestObjects;

namespace M2SA.AppGenome.Queues.Tests
{
    [TestFixture]
    public class MultiNodeClusterTest : TestBase
    {
        private string queueName = "testClusterC";

        [Test]
        public void SendTest()
        {
            var times = new Random().Next(3, 10);
            for (var i = 0; i < times; i++)
            {
                var message = new TestMessage()
                {
                    Name = new Random().Next(10000).ToString()
                };
                TestHelper.SendTest(queueName, message);
            }
        }

        [Test]
        public void ReceiveTest()
        {
            var times = new Random().Next(3, 10);
            for (var i = 0; i < times; i++)
            {
                var message = new TestMessage()
                {
                    Name = new Random().Next(10000).ToString()
                };
                TestHelper.ReceiveTest(queueName, message);
            }            
        }

        [Test]
        public void BeginReceiveTest()
        {
            var times = new Random().Next(3, 10);
            for (var i = 0; i < times; i++)
            {
                var message = new TestMessage()
                {
                    Name = new Random().Next(10000).ToString()
                };
                TestHelper.BeginReceiveTest(queueName, message);
            }
        }
    }
}

