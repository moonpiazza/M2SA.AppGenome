using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using M2SA.AppGenome.Configuration;

namespace M2SA.AppGenome.Logging.Listeners
{
    /// <summary>
    /// 
    /// </summary>
    public interface IListener : IResolveObject
    {
        /// <summary>
        /// 
        /// </summary>
        string Name { get; }
        /// <summary>
        /// 
        /// </summary>
        string Type { get; }
        /// <summary>
        /// 
        /// </summary>
        bool SupportAsync { get; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="entry"></param>
        void WriteMessage(ILogEntry entry);
    }
}
