using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Web;
using System.Collections;
using M2SA.AppGenome.Logging;

namespace M2SA.AppGenome.Web
{
    /// <summary>
    /// 
    /// </summary>
    public class AppGenomeModule : IHttpModule
    {
        HttpApplication httpApplication;
        Stopwatch stopwatch = null;

        #region IHttpModule 成员

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        public void Init(HttpApplication context)
        {
            ArgumentAssertion.IsNotNull(context, "context");
            this.httpApplication = context;
            this.httpApplication.Error += new EventHandler(context_Error);
            this.httpApplication.BeginRequest += new EventHandler(httpApplication_BeginRequest);
            this.httpApplication.EndRequest += new EventHandler(httpApplication_EndRequest);
        }

        #endregion

        void httpApplication_EndRequest(object sender, EventArgs e)
        {
            if (AppInstance.Config.Debug)
            {
                this.stopwatch.Stop();
                LogManager.GetLogger().Trace("{0} : {1}", this.httpApplication.Context.Request.Url, this.stopwatch.Elapsed);
            }
         }

        void httpApplication_BeginRequest(object sender, EventArgs e)
        {
            if (AppInstance.Config.Debug)
            {
                this.stopwatch = Stopwatch.StartNew();
            }
        }

        void context_Error(object sender, EventArgs e)
        {            
            Exception source = this.httpApplication.Server.GetLastError();
            if (ValidateRequest(this.httpApplication.Context.Request))
            {
                var ex = new HttpUnhandledException(source.Message, source);
                ex.HandleException();
            }
            else
            {
                var msg = string.Format("[HostileRequest]{0}", source.Message);
                var ex = new HostileRequestException(msg, source);
                ex.HandleException();
            }
        }

        static bool ValidateRequest(HttpRequest request)
        {
            var isValidate = true;
            var isCrawlerRequest = HttpValidator.IsCrawlerRequest(request);

            if (isCrawlerRequest == false)
                isValidate = !HttpValidator.ExceededMaximumRequestLength(request);
         
            return isValidate;
        }
    }
}
