using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using M2SA.AppGenome.Queues;

namespace M2SA.AppGenome.Logging.Listeners
{
    /// <summary>
    /// 
    /// </summary>
    public class QueueListener : ListenerBase
    {
        /// <summary>
        /// 
        /// </summary>
        public string Queue { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public QueueListener()
        {
            this.SupportAsync = false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entry"></param>
        public override void WriteMessage(ILogEntry entry)
        {
            QueueManager.SendToQueue(entry, this.Queue);
        }
    }
}
