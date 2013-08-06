using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace M2SA.AppGenome.Logging
{
    /// <summary>
    /// 
    /// </summary>
    public class LevelConstraint
    {
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
        public LogLevel SysInfoLimit
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public LevelConstraint()
        {
            this.MinLevel = LogLevel.Info;
            this.MaxLevel = LogLevel.Off;
            this.SysInfoLimit = LogLevel.Off;

            if (AppInstance.Config.Debug)
            {
                this.MinLevel = LogLevel.All;
            }
        }
    }
        
}
