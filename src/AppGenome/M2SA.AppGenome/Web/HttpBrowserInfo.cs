using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace M2SA.AppGenome.Web
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class HttpBrowserInfo
    {
        /// <summary>
        /// 
        /// </summary>
        public string Browser
        {
            get;
            set;
        }        

        /// <summary>
        /// 
        /// </summary>
        public string BrowserType
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public string Platform
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsMobileDevice
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public bool Crawler
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public int MajorVersion
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public double MinorVersion
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="browser"></param>
        public HttpBrowserInfo(HttpBrowserCapabilities browser)
        {
            if (null == browser)
                return;
            this.Browser = browser.Browser;
            this.Browser = browser.Type;
            this.Platform = browser.Platform;
            this.IsMobileDevice = browser.IsMobileDevice;
            this.Crawler = browser.Crawler;
            this.MajorVersion = browser.MajorVersion;
            this.MinorVersion = browser.MinorVersion;
        }
    }
}
