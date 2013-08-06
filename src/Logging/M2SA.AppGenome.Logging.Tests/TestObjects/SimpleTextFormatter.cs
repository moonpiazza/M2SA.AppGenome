using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using M2SA.AppGenome.Logging.Formatters;
namespace M2SA.AppGenome.Logging.Tests.TestObjects
{
    public class SimpleTextFormatter : ILogEntryFormatter
    {
        public readonly static string Templete = "@SessionId@Message";

        #region ILogEntryFormatter 成员

        public object Format(ILogEntry entry)
        {
            if (entry is ISessionEntry)
            {
                var sessionEntry = (entry as ISessionEntry);
                return this.FormatEntry(sessionEntry);
            }
            else
            {
                return this.FormatEntry(entry);
            }
        }

        #endregion

        private object FormatEntry(ILogEntry entry)
        {
            return Templete.Replace("@SessionId", entry.SessionId).Replace("@Message", entry.Message);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entry"></param>
        /// <returns></returns>
        private object FormatEntry(ISessionEntry entry)
        {
            var builder = new StringBuilder(512 * (entry.EntryList.Count + 1));

            for (var i = 0; i < entry.EntryList.Count; i++)
            {
                builder.Append(FormatEntry(entry.EntryList[i]));
            }
            return builder.ToString();
        }

    }
}
