using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.IO;
using NUnit.Framework;
using M2SA.AppGenome.Logging.Listeners;
using M2SA.AppGenome.Logging.Formatters;

namespace M2SA.AppGenome.Logging.Tests
{
    /// <summary>
    /// 
    /// </summary>
    /// <include file='appgenome.config' />
    [TestFixture]
    public class FileLogTest : TestBase
    {
        [Test]
        public void TestLogInfo()
        {
            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs\\log-file-A.txt");
            if (File.Exists(filePath))
                File.Delete(filePath);

            var now = DateTime.Now;
            Console.WriteLine(now.ToString("bdvvss"));


            var groupName = "FileTestA";
            var source = new string[] { "a1", "b1", "c1" };

            var log = LogManager.GetLogger(groupName);
            for (var i = 0; i < source.Length; i++)
            {
                log.Info(source[i]);
            }

            Thread.Sleep(2000);
            Assert.IsTrue(File.Exists(filePath));            
        }

        [Test]
        public void TestMutiThread()
        {
            var m = new Random().Next(3, 6);
            for (var i = 0; i < m; i++)
            {
                AppInstance.GetThreadPool().QueueWorkItem<string>((x) => TestExInfo(), string.Empty);
            }

            Thread.Sleep(10000);
        }

        [Test]
        public void TestExInfo()
        {
            var groupName = "FileTestA";
            try
            {
                var n = int.Parse("a");
            }
            catch (Exception ex)
            {
                LogManager.GetLogger(groupName).Error(ex, null);
            }
        }
    }
}
