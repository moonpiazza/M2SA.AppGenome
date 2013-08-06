using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace M2SA.AppGenome.Logging.Formatters
{
    /// <summary>
    /// Serializes ILogEntry 
    /// </summary>
    public interface ILogEntryFormatter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="entry"></param>
        /// <returns></returns>
        object Format(ILogEntry entry);
    }
}
