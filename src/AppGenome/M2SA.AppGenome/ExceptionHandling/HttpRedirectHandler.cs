using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using M2SA.AppGenome.Configuration;
using M2SA.AppGenome.Reflection;

namespace M2SA.AppGenome.ExceptionHandling
{
    /// <summary>
    /// 
    /// </summary>
    public class HttpRedirectHandler : IExceptionHandler
    {
        static readonly string MesssageKey = "@Message";

        /// <summary>
        /// 
        /// </summary>
        public string Url
        {
            get;
            private set;
        }

        #region IExceptionHandler 成员

        /// <summary>
        /// 
        /// </summary>
        /// <param name="exception"></param>
        /// <param name="handlingInstanceId"></param>
        /// <param name="bizInfo"></param>
        /// <returns></returns>
        public Exception HandleException(Exception exception, Guid handlingInstanceId, IDictionary bizInfo)
        {
            ArgumentAssertion.IsNotNull(exception, "exception");

            var context = HttpContext.Current;
            if (context == null)
                return exception;

            if (string.IsNullOrEmpty(this.Url))
            {
                throw new ConfigException("node define the Redirect Url");
            }

            var rawUrl = this.Url;
            if (rawUrl.Contains(MesssageKey))
                rawUrl = rawUrl.Replace(MesssageKey, exception.Message);

            context.Response.Redirect(rawUrl);

            return exception;
        }

        #endregion
    }
}
