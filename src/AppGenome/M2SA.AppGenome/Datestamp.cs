using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace M2SA.AppGenome
{
    /// <summary>
    /// 
    /// </summary>
    public static class Datestamp
    {
        /// <summary>
        /// 
        /// </summary>
        public static readonly DateTime ZeroTime = new DateTime(1911, 10, 10);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static int ToDatestamp(DateTime dateTime)
        {
            TimeSpan ts = (dateTime.ToUniversalTime() - ZeroTime);
            int datestamp = (int)ts.TotalDays;
            return datestamp;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="datestamp"></param>
        /// <returns></returns>
        public static DateTime ToDateTime(int datestamp)
        {
            var ts = TimeSpan.FromDays(datestamp);
            var dateTime = ZeroTime.Add(ts);
            return dateTime.ToLocalTime();
        }
    }
}
