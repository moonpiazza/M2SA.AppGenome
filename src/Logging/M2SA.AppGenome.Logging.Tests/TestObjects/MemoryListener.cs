using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using M2SA.AppGenome.Logging.Listeners;
using M2SA.AppGenome.Logging.Formatters;

namespace M2SA.AppGenome.Logging.Tests.TestObjects
{
    public class MemoryListener : ListenerBase
    {
        public string Source
        {
            get;
            private set;
        }

        public ILogEntryFormatter Formatter
        {
            get;
            private set;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entry"></param>
        public override void WriteMessage(ILogEntry entry)
        {
            var info = this.Formatter.Format(entry);
            ObjectIOCFactory.GetSingleton<MemoryLogSource>().Store(this.Source, info);
        }
    }
}
