using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace M2SA.AppGenome.Configuration
{
    /// <summary>
    /// 
    /// </summary>
    public class ThreadPoolConfig
    {
        /// <summary>
        /// 
        /// </summary>
        public int MinThreads
        {
            get;
            private set;
        }

        /// <summary>
        /// 
        /// </summary>
        public int MaxThreads
        {
            get;
            private set;
        }

        /// <summary>
        /// 
        /// </summary>
        public int IdleTimeout
        {
            get;
            private set;
        }

        /// <summary>
        /// 
        /// </summary>
        public ThreadPoolConfig()
        {
            this.MinThreads = 0;
            this.MaxThreads = 2;
            this.IdleTimeout = 300 * 1000;
        }
    }
}
