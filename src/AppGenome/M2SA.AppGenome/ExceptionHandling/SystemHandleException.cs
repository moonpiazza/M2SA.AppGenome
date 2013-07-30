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
    public class SystemHandleException : IExceptionPolicy
    {
        #region IExceptionPolicy 成员

        /// <summary>
        /// 
        /// </summary>
        /// <param name="originalException"></param>
        /// <param name="bizInfo"></param>
        /// <returns></returns>
        public bool HandleException(Exception originalException, IDictionary bizInfo)
        {
            LogManager.GetLogger().Error(originalException, bizInfo);
            throw originalException;
        }

        #endregion

        #region IResolveObject 成员

        /// <summary>
        /// 
        /// </summary>
        /// <param name="config"></param>
        public void Initialize(IConfigNode config)
        {
            //empty action
        }

        #endregion
    }
}
