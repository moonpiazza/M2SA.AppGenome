using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace M2SA.AppGenome.Logging.Listeners
{
    /// <summary>
    /// 
    /// </summary>
    public class ListenerProxy
    {
        /// <summary>
        /// 
        /// </summary>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public bool Enabled
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public LogLevel MinLevel
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public LogLevel MaxLevel
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public ListenerProxy()
        {
            this.Enabled = true;
            this.MinLevel = LogLevel.All;
            this.MaxLevel = LogLevel.Off;
        }
    }
}
