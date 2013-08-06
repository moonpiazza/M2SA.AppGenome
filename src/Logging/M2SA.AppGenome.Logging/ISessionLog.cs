using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace M2SA.AppGenome.Logging
{
    /// <summary>
    /// 
    /// </summary>
    public interface ISessionLog : ILog, IDisposable
    {
        /// <summary>
        /// 
        /// </summary>
        string SessionId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="now"></param>
        void Flush(DateTime now);
    }
}
