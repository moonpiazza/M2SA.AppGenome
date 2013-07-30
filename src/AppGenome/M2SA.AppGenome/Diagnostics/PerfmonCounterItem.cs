using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace M2SA.AppGenome.Diagnostics
{
    /// <summary>
    /// 
    /// </summary>
    public class PerfmonCounterItem
    {
        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public bool ReadOnly { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public long RawValue { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="readOnly"></param>
        public PerfmonCounterItem(bool readOnly)
        {
            this.ReadOnly = readOnly;
            this.RawValue = 0;
        }        
    }
}
