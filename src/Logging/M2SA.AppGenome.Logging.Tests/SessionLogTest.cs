using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.IO;
using NUnit.Framework;
using M2SA.AppGenome.Logging.Listeners;
using M2SA.AppGenome.Reflection;
using M2SA.AppGenome.Logging.Tests.TestObjects;

namespace M2SA.AppGenome.Logging.Tests
{
    /// <summary>
    /// 
    /// </summary>
    /// <include file='appgenome.config' />
    [TestFixture]
    public class SessionLogTest : TestBase
    {
        public override void Start()
        {
            base.Start();
            AppInstance.RegisterTypeAliasByModule<MemoryListener>(AppConfig.LoggingKey);
            AppInstance.RegisterTypeAliasByModule<SimpleTextFormatter>(AppConfig.LoggingKey);
        }

        [Test]
        public void GetLoggerTest()
        {
            var sessionLogName = "SessionTestA";
            var memoryLogName = "MemoryTestA";
            var logA = LogManager.GetLogger(memoryLogName);
            var logB = LogManager.GetLogger(memoryLogName);
            Assert.AreEqual(logA, logB);


            logA = LogManager.GetLogger(sessionLogName);
            logB = LogManager.GetLogger(sessionLogName);
            Assert.AreNotEqual(logA, logB);

            var sid = Guid.NewGuid().ToString();

            logA = LogManager.GetSessionLogger(sessionLogName, sid);
            logB = LogManager.GetSessionLogger(sessionLogName, sid);
            Assert.AreEqual(logA, logB);

            logB = LogManager.GetSessionLogger(sessionLogName, Guid.NewGuid().ToString());
            Assert.AreNotEqual(logA, logB);

        }

        [Test]
        public void LogInfoTest()
        {
             var sessionLogName = "SessionTestA";
             ObjectIOCFactory.GetSingleton<MemoryLogSource>().Clear(sessionLogName);

           
            var sourceA = new string[] { "a123456789", "b123456789", "c123456789" };
            var sourceB = new string[] { "X987654321", "Y987654321", "Z987654321" };
            var idA= Guid.NewGuid().ToString();
            var idB = Guid.NewGuid().ToString();

            var logA = LogManager.GetSessionLogger(sessionLogName, idA);
            var logB = LogManager.GetSessionLogger(sessionLogName, idB);
            for (var i = 0; i < sourceA.Length; i++)
            {
                var msgA = sourceA[i];                
                new Thread(new ThreadStart(() => logA.Info(msgA))).Start();

                var msgB = sourceB[i];
                new Thread(new ThreadStart(() => logB.Info(msgB))).Start();
            }

            Thread.Sleep(1500);

            ((ISessionLog)logB).Dispose(); //logB先释放资源, 会先写入写入连续日志信息
            ((ISessionLog)logA).Dispose();            

            var logData = ObjectIOCFactory.GetSingleton<MemoryLogSource>().GetLogInfos(sessionLogName);
            Assert.AreEqual(2, logData.Count);

            var logDataB = (string)logData[0];
            Assert.IsTrue(logDataB.Contains(sourceB[sourceB.Length - 1]));
            Assert.IsFalse(logDataB.Contains(sourceA[0]));

            var logDataA = (string)logData[1];
            Assert.IsTrue(logDataA.Contains(sourceA[0]));
            Assert.IsFalse(logDataA.Contains(sourceB[0]));

        }
    }
}
