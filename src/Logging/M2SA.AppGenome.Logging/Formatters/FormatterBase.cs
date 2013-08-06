using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace M2SA.AppGenome.Logging.Formatters
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class FormatterBase : ILogEntryFormatter
    {
        #region ILogEntryFormatter 成员

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entry"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entry"></param>
        /// <returns></returns>
        protected abstract object FormatEntry(ILogEntry entry);
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="entry"></param>
        /// <returns></returns>
        protected abstract object FormatEntry(ISessionEntry entry);
    }
}
