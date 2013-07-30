using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace M2SA.AppGenome.Diagnostics
{
    /// <summary>
    /// 
    /// </summary>
    public class PerfmonCounter
    {
        /// <summary>
        /// 
        /// </summary>
        public string InstanceName { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public string CategoryName { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public IDictionary<string, PerformanceCounter> CounterData { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="instanceName"></param>
        /// <param name="categoryName"></param>
        public PerfmonCounter(string instanceName, string categoryName)
        {
            this.InstanceName = instanceName;
            this.CategoryName = categoryName;

            this.CounterData = new Dictionary<string, PerformanceCounter>();
        }        
    }
}
