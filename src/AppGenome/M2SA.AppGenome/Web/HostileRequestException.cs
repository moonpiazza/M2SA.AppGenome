using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Web;

namespace M2SA.AppGenome.Web
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class HostileRequestException : Exception
    {
            /// <summary>
            /// 
            /// </summary>
            /// <param name="info"></param>
            /// <param name="context"></param>
            protected HostileRequestException(SerializationInfo info, StreamingContext context)
                : base(info, context)
            {
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="message"></param>
            /// <param name="innerException"></param>
            public HostileRequestException(string message, Exception innerException)
                : base(message, innerException)
            {
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="innerException"></param>
            public HostileRequestException(Exception innerException)
                : base(null == innerException?"":innerException.Message, innerException)
            {
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="message"></param>
            public HostileRequestException(string message)
                : base(message)
            {
            }

            /// <summary>
            /// 
            /// </summary>
            public HostileRequestException()
                : base()
            {
            }
    }
}
