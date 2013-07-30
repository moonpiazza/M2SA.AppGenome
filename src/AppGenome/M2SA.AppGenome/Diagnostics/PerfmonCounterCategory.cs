using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace M2SA.AppGenome.Diagnostics
{
    /// <summary>
    /// 
    /// </summary>
    public class PerfmonCounterCategory
    {
        /// <summary>
        /// 
        /// </summary>
        public string MachineName { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public string CategoryName { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public IDictionary<string, PerfmonCounter> Instances { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="categoryName"></param>
        /// <param name="machineName"></param>
        public PerfmonCounterCategory(string categoryName, string machineName)
        {
            this.MachineName = machineName;
            this.CategoryName = categoryName;

            this.Instances = new Dictionary<string, PerfmonCounter>(2);
        }
    }
}
