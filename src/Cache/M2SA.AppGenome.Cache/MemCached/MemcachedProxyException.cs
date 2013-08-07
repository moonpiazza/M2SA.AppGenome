using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace M2SA.AppGenome.Cache.MemCached
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
	public class MemcachedProxyException : Exception
	{
        /// <summary>
        /// 
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected MemcachedProxyException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public MemcachedProxyException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="innerException"></param>
        public MemcachedProxyException(Exception innerException)
            : base(null == innerException ? "" : innerException.Message, innerException)
        {            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public MemcachedProxyException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public MemcachedProxyException()
            : base()
        {
        }
	}
}