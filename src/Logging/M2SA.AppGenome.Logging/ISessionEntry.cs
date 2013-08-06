using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace M2SA.AppGenome.Logging
{
    /// <summary>
    /// 
    /// </summary>
    public interface ISessionEntry : ILogEntry
    {
        /// <summary>
        /// 
        /// </summary>
        IList<ILogEntry> EntryList { get; }
        
        /// <summary>
        /// 
        /// </summary>
        DateTime CreateTime { get; }
        
        /// <summary>
        /// 
        /// </summary>
        DateTime StartTime
        {
            get;
        }

        /// <summary>
        /// 
        /// </summary>
        DateTime FlushTime
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entry"></param>
        void AppendEntry(ILogEntry entry);
    }
}
