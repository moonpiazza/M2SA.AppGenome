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
    [TestFixture]
    class FormatterUtilityTest
    {
        [Test]
        public void ParseKeyTest()
        {
            var text = "---AppName:@AppName  -- @SessionId \r\n";    
            var result = FormatterUtility.ParseTextToKeyList(text);
            Assert.AreEqual(2, result.Count);
        }

        [Test]
        public void FormatTest()
        {
            var logLevel = LogLevel.Debug;
            var message = TestHelper.RandomizeString();
            var entry = TestHelper.CreateEntry(logLevel, message, null);

            var text = "---AppName:@LogLevel  -- @Message \r\n";  
            var expected = text.Replace("@LogLevel", logLevel.ToString()).Replace("@Message", message);
            var actual = FormatterUtility.Format(entry, text);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ToMapTest()
        {
            var logLevel = LogLevel.Debug;
            var message = TestHelper.RandomizeString();
            var entry = TestHelper.CreateEntry(logLevel, message, null);
            
            var text = "---AppName:@LogLevel  -- @Message \r\n";
            var result = FormatterUtility.ToMap(entry, text);
            Assert.AreEqual(2, result.Count);
        }
    }
}
