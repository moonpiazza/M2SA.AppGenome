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
    public class HttpCookieInfo 
    {
        /// <summary>
        /// 
        /// </summary>
        public string Domain
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public DateTime Expires
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public string Path
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public bool Secure
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public bool HttpOnly
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public string Value
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public IDictionary<string,string> Values
        {
            get;
            private set;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cookie"></param>
        public HttpCookieInfo(HttpCookie cookie)
        {
            ArgumentAssertion.IsNotNull(cookie, "cookie");

            this.Name = cookie.Name;
            this.Domain = cookie.Domain;
            this.Expires = cookie.Expires;
            this.Path = cookie.Path;
            this.HttpOnly = cookie.HttpOnly;
            this.Secure = cookie.Secure;
            if (cookie.HasKeys)
            {
                this.Values = new Dictionary<string, string>(cookie.Values.Count);
                foreach (var itemKey in cookie.Values.AllKeys)
                {
                    if (cookie.Values[itemKey] != null)
                    {
                        this.Values.Add(itemKey, cookie.Values[itemKey]);
                    }
                }
            }
            else
                this.Value = cookie.Value;
        }
    }
}
