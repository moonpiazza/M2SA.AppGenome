using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using M2SA.AppGenome.Logging.Listeners;
using M2SA.AppGenome.Logging.Tests.TestObjects;

namespace M2SA.AppGenome.Logging.Tests
{
    /// <summary>
    /// 
    /// </summary>
    /// <include file='appgenome.config' />
    [TestFixture]
    public class MemoryLogTest : TestBase
    {
        public override void Start()
        {
            base.Start();
            AppInstance.RegisterTypeAliasByModule<MemoryListener>(AppConfig.LoggingKey);
            AppInstance.RegisterTypeAliasByModule<SimpleTextFormatter>(AppConfig.LoggingKey);            
        }

        [Test]
        public void LogInfoTest()
        {
            var loggerName = "MemoryTestA";
            var source = new string[] { "a1", "b1", "c1" };

            var log = LogManager.GetLogger(loggerName);
            {
                for (var i = 0; i < source.Length; i++)
                {
                    log.Info(source[i]);
                }
            }

            var logData = ObjectIOCFactory.GetSingleton<MemoryLogSource>().GetLogInfos(loggerName);
            Assert.AreEqual(source.Length, logData.Count);
            for (var i = 0; i < source.Length; i++)
            {
                Assert.AreEqual(source[i], logData[i]);
            }
        }
    }
}
