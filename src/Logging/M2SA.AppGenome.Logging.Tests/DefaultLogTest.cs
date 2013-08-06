using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Diagnostics;
using System.IO;
using NUnit.Framework;
using M2SA.AppGenome.Reflection;

namespace M2SA.AppGenome.Logging.Tests
{
    /// <summary>
    /// 
    /// </summary>
    /// <include file='appgenome.config'/>
    [TestFixture]
    public class DefaultLogTest : TestBase
    {
        [Test]
        public void TestLogInfo()
        {
            var log = LogManager.GetLogger();

            var dic = new Dictionary<string, object>();
            dic.Add("Method", "TestLogInfo");
            dic.Add("BizType", 111);
            dic.Add("BizId", 111.ToString());
            dic.Add("Message", "ABVC");
            log.Info(dic);            
        }
    }
}
