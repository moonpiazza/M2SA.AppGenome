using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace M2SA.AppGenome.Reflection
{
    /// <summary>
    /// PropertyAccessorException class.
    /// </summary>
    [Serializable]
    public class PropertyAccessorException : Exception
    {
        /// <summary>
        /// 
        /// </summary>
        public PropertyAccessorException()
            : base()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public PropertyAccessorException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public PropertyAccessorException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="si"></param>
        /// <param name="sc"></param>
        protected PropertyAccessorException(SerializationInfo si, StreamingContext sc)
            : base(si, sc)
        {
        }
    }
}
