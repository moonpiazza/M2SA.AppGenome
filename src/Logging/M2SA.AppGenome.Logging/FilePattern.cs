using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace M2SA.AppGenome.Logging
{
    /// <summary>
    /// 
    /// </summary>
    public class FilePattern
    {
        /// <summary>
        /// 
        /// </summary>
        public string PathPattern
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public int TryTimes
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public FilePattern()
        {
            this.TryTimes = 3;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entry"></param>
        /// <returns></returns>
        public string GetFileName(ILogEntry entry)
        {
            if (null == entry)
                throw new ArgumentNullException("entry");

            var logTime = entry.LogTime;
            if (entry is ISessionEntry)
            {
                logTime = (entry as ISessionEntry).FlushTime;
            }
            var filePath = this.PathPattern;
            filePath = filePath.Replace("YYYY", logTime.ToString("yyyy")).Replace("MM", logTime.ToString("MM"));
            filePath = filePath.Replace("DD", logTime.ToString("dd")).Replace("HH", logTime.ToString("HH"));
            filePath = filePath.Replace("mm", logTime.ToString("mm"));
            if (string.IsNullOrEmpty(entry.SessionId) == false)
            {
                filePath = filePath.Replace("@SessionId", entry.SessionId);
            }

            return filePath;
        } 
    }
}
