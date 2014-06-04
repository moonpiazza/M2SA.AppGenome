using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace M2SA.AppGenome.Data.SqlMap
{
    /// <summary>
    /// 
    /// </summary>
    public class KeywordProcessResult
    {
        /// <summary>
        /// 
        /// </summary>
        public bool IsMatch { get; internal set; }

        /// <summary>
        /// 
        /// </summary>
        public string SqlExpression { get; internal set; }

        /// <summary>
        /// 
        /// </summary>
        public IDictionary<string, object> ParameterValues { get; internal set; }
    }
}
