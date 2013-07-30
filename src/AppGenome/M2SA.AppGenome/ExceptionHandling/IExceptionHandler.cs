using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace M2SA.AppGenome.ExceptionHandling
{
    /// <summary>
    /// 
    /// </summary>
    public interface IExceptionHandler
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="exception"></param>
        /// <param name="handlingInstanceId"></param>
        /// <param name="bizInfo"></param>
        /// <returns></returns>
        Exception HandleException(Exception exception, Guid handlingInstanceId, IDictionary bizInfo);
    }
}
