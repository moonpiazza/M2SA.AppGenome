using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace M2SA.AppGenome.Logging
{
    /// <summary>
    /// 
    /// </summary>
    public class SessionLogger : Logger, ISessionLog
    {
        /// <summary>
        /// 
        /// </summary>
        public DateTime processTime { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ISessionEntry SessionEntry { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string SessionId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int BufferSize { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public SessionLogger()
        {
            this.SessionEntry = new SessionEntry();            
            this.ProcessInterval = TimeSpan.FromMinutes(1);
            this.BufferSize = 100;

            this.processTime = DateTime.Now;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entry"></param>
        protected override void WriteAsyncMessage(ILogEntry entry)
        {
            if (null == entry)
                throw new ArgumentNullException("entry");

            this.SessionEntry.AppendEntry(entry);
            if (this.SessionEntry.EntryList.Count == 1)
            {
                this.SessionEntry.AppName = entry.AppName;
                this.SessionEntry.SessionId = this.SessionId;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entry"></param>
        protected override void WriteAsyncMessageByThread(ILogEntry entry)
        {
            var now = DateTime.Now;
            for (var i = 0; i < this.SessionEntry.EntryList.Count; i++)
            {
                this.SessionEntry.EntryList[i].WriteTime = now;
            }

            base.WriteAsyncMessageByThread(entry);
            this.SessionEntry.EntryList.Clear();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override ILogEntry CreateEntry()
        {
            var entry = base.CreateEntry();
            entry.SessionId = this.SessionId;
            return entry;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="now"></param>
        public void Flush(DateTime now)
        {
            var isFlush = this.SessionEntry.EntryList.Count >= this.BufferSize;
            if (isFlush == false)
                isFlush = (now - this.processTime) > this.ProcessInterval;

            if (isFlush)
            {
                lock (SyncObject)
                {
                    var count = this.SessionEntry.EntryList.Count;
                    this.SessionEntry.FlushTime = now;
                    this.WriteAsyncMessageByThread(this.SessionEntry);
                    this.processTime = DateTime.Now;
                    Console.WriteLine("Flush[{0}]:{1}->{2}", this.SessionId, count, this.SessionEntry.EntryList.Count);                    
                }
            }
        }

        #region IDisposable 

        /// <summary>
        /// 
        /// </summary>
        ~SessionLogger() 
        { 
            Dispose(false); 
        }

        void Dispose(bool disposing)
        {
            this.SessionEntry.FlushTime = DateTime.Now;
            this.WriteAsyncMessageByThread(this.SessionEntry);
            if (disposing)
            {                
                GC.SuppressFinalize(this);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            Dispose(true); 
        }

        #endregion
    }
}
