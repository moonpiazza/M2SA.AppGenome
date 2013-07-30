using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using M2SA.AppGenome.Configuration;

namespace M2SA.AppGenome.ExceptionHandling
{
    /// <summary>
    /// 
    /// </summary>
    public interface IExceptionPolicy : IResolveObject
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="originalException"></param>
        /// <param name="bizInfo"></param>
        /// <returns></returns>
        bool HandleException(Exception originalException, IDictionary bizInfo);
    }
}
