using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using M2SA.AppGenome.Reflection;

namespace M2SA.AppGenome.Logging
{
    /// <summary>
    /// 
    /// </summary>
    public static class EffectiveFileLogger
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ex"></param>
        public static void WriteException(Exception ex)
        {
            var exLog = string.Format("{0}_LoggingException.log", DateTime.Now.ToString("yyyyMMddHH"));
            var exInfo = ex.ToText();
            FileHelper.WriteInfo(exLog, exInfo);

            Console.WriteLine("*******************LoggingException*******************");
            Console.WriteLine(exInfo);
            Console.WriteLine("******************************************************");
        }
    }
}
