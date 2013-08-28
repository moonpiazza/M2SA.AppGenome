using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace M2SA.AppGenome.Logging
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class SessionEntry : LogEntry, ISessionEntry
    {
        #region ISessionEntry 成员

        /// <summary>
        /// 
        /// </summary>
        public IList<ILogEntry> EntryList
        {
            get;
            private set;
        }

        /// <summary>
        /// 
        /// </summary>
        public DateTime CreateTime
        {
            get;
            private set;
        }

        /// <summary>
        /// 
        /// </summary>
        public DateTime StartTime
        {
            get;
            private set;
        }

        /// <summary>
        /// 
        /// </summary>
        public DateTime FlushTime
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public override LogLevel LogLevel
        {
            get
            {
                base.LogLevel = LogLevel.All;
                if (this.EntryList.Count > 0)
                {
                    for (var i = 0; i < this.EntryList.Count; i++)
                    {
                        if (base.LogLevel < this.EntryList[i].LogLevel)
                            base.LogLevel = this.EntryList[i].LogLevel;
                    }
                }

                return base.LogLevel;
            }
            set
            {
                base.LogLevel = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entry"></param>
        public void AppendEntry(ILogEntry entry)
        {
            if (this.StartTime == DateTime.MaxValue)
            {
                this.StartTime = DateTime.Now;
                this.LogTime = this.StartTime;
            }

            this.EntryList.Add(entry);
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        public SessionEntry()
        {
            this.CreateTime = DateTime.Now;
            this.StartTime = DateTime.MaxValue;
            this.EntryList = new List<ILogEntry>();
        }
    }
}
