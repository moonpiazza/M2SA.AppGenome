using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace M2SA.AppGenome
{
    /// <summary>
    /// 
    /// </summary>
    public static class Timestamp
    {
        /// <summary>
        /// 
        /// </summary>
        public static readonly DateTime ZeroTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static long ToTimestamp(DateTime dateTime)
        {
            TimeSpan ts = (dateTime.ToUniversalTime() - ZeroTime);
            long timestamp = (long)ts.TotalSeconds;
            return timestamp;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="timestamp"></param>
        /// <returns></returns>
        public static DateTime ToDateTime(long timestamp)
        {
            var ts = TimeSpan.FromSeconds(timestamp);
            var dateTime = ZeroTime.Add(ts);
            return dateTime.ToLocalTime();
        }

    }
}
