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
        public override LogLevel LogLeveL
        {
            get
            {
                base.LogLeveL = LogLevel.All;
                if (this.EntryList.Count > 0)
                {
                    for (var i = 0; i < this.EntryList.Count; i++)
                    {
                        if (base.LogLeveL > this.EntryList[i].LogLeveL)
                            base.LogLeveL = this.EntryList[i].LogLeveL;
                    }
                }

                return base.LogLeveL;
            }
            set
            {
                base.LogLeveL = value;
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
