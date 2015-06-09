using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using M2SA.AppGenome.Configuration;
using M2SA.AppGenome.Logging.Formatters;

namespace M2SA.AppGenome.Logging.Listeners
{
    /// <summary>
    /// 
    /// </summary>
    public class ConsoleListener : ListenerBase
    {
        /// <summary>
        /// 
        /// </summary>
        public ILogEntryFormatter Formatter
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entry"></param>
        public override void WriteMessage(ILogEntry entry)
        {
            var message = this.Formatter.Format(entry);
            SystemLogger.WriteLine(entry.LogLevel, message.ToString());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="config"></param>
        public override void Initialize(IConfigNode config)
        {
            base.Initialize(config);
            if (this.Formatter == null)
            {
                this.Formatter = new TextFormatter();
            }
        }
    }
}
