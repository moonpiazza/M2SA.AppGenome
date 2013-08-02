using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Web;
using System.Collections;
using System.Configuration;
using System.Web.Configuration;

namespace M2SA.AppGenome.Web
{
    /// <summary>
    /// 
    /// </summary>
    public class HttpValidator
    {
        static int MaxRequestLength = 0; // KB

        /// <summary>
        /// 是否是爬虫请求
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        public static bool IsCrawlerRequest(HttpRequest request)
        {
            ArgumentAssertion.IsNotNull(request, "request");

            var isCrawler = false;
            try
            {
                var browserInfo = request.Browser;
                if (browserInfo != null && browserInfo.Type != null && browserInfo.Crawler == false)
                {
                    var httpUserAgent = string.Empty;
                    if (request.QueryString.AllKeys.Contains("HTTP_USER_AGENT"))
                        httpUserAgent = request.QueryString["HTTP_USER_AGENT"];
                    else if (request.ServerVariables.AllKeys.Contains("HTTP_USER_AGENT") && null != request.ServerVariables["HTTP_USER_AGENT"])
                        httpUserAgent = request.ServerVariables["HTTP_USER_AGENT"];

                    if (httpUserAgent.Length < 15)     //  "Mozilla/4.0 (co"
                        isCrawler = true;
                }
            }
            catch
            {
                isCrawler = false;
            }
            return isCrawler;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static int GetMaxRequestLength()
        {
            if (MaxRequestLength == 0)
            {
                var httpRuntimeInfo = ConfigurationManager.GetSection("system.web/httpRuntime");
                if (httpRuntimeInfo != null)
                {
                    MaxRequestLength = ((HttpRuntimeSection)httpRuntimeInfo).MaxRequestLength * 1024;
                }    
            }
            return MaxRequestLength;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        public static bool ExceededMaximumRequestLength(HttpRequest request)
        {
            ArgumentAssertion.IsNotNull(request, "request");

            var exceeded = false;
            try
            {
                var httpContentLength = 0;
                if (request.ServerVariables.AllKeys.Contains("HTTP_CONTENT_LENGTH") && null != request.ServerVariables["HTTP_CONTENT_LENGTH"])
                    httpContentLength = Convert.ToInt32(request.ServerVariables["HTTP_CONTENT_LENGTH"]);

                var maxLengthLimit = GetMaxRequestLength();
                if (httpContentLength > 0 && maxLengthLimit > 0 && httpContentLength >= maxLengthLimit)
                    exceeded = true;
            }
            catch
            {
                exceeded = false;
            }

            return exceeded;
        }
    }
}
