using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Diagnostics;
using System.Threading;
using NUnit.Framework;
using M2SA.AppGenome.Diagnostics;
using M2SA.AppGenome.Threading;
using M2SA.AppGenome.Configuration;
using M2SA.AppGenome.Reflection;
using System.Reflection;


namespace M2SA.AppGenome.Logging.Tests
{
    public static class TestHelper
    {
        public static bool RandomizeBoolean()
        {
            var randomNumber = new Random().Next(0, 2);
            return randomNumber == 1 ;
        }

        public static int RandomizeInt()
        {
            return RandomizeNumber();
        }

        public static string RandomizeString()
        {
            return RandomizeString(string.Empty);
        }

        public static string RandomizeString(string prefixion)
        {
            if (null == prefixion)
                prefixion = string.Empty;

            return string.Concat(prefixion, RandomizeNumber());
        }

        private static int RandomizeNumber()
        {
            Thread.Sleep(100);

            var number = new Random().Next(1000, 9000);
            return number;
        }


        public static ILogEntry CreateEntry(LogLevel level, object message, Exception exception)
        {
            var log = new Logger();
            var argDefines = new Type[] { typeof(LogLevel), typeof(object), typeof(Exception) };
            var method = typeof(Logger).GetMethod("CreateEntry", BindingFlags.Instance | BindingFlags.NonPublic, null, argDefines, null);
            var args = new object[] { level, message, exception };
            var obj = method.Invoke(new Logger(), args);
            return (ILogEntry)obj;
        }
    }
}
