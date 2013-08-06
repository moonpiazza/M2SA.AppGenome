using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using M2SA.AppGenome.Reflection;

namespace M2SA.AppGenome.Logging.Formatters
{
    /// <summary>
    /// 
    /// </summary>
    public class TextFormatter : FormatterBase
    {
        /// <summary>
        /// 
        /// </summary>
        public string Header
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public string Footer
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public string Content
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public TextFormatter()
        {
            
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="entry"></param>
        /// <returns></returns>
        protected override object FormatEntry(ILogEntry entry)
        {
            var builder = new StringBuilder(512);

            if (string.IsNullOrEmpty(this.Header) == false)
                builder.Append(FormatterUtility.Format(entry, this.Header));

            if (string.IsNullOrEmpty(this.Content))
                builder.Append(entry.ToText());
            else
                builder.Append(FormatterUtility.Format(entry, this.Content));

            if (string.IsNullOrEmpty(this.Footer) == false)
                builder.Append(FormatterUtility.Format(entry, this.Footer));

            builder.Replace(@"\r\n", "\r\n");
            builder.Replace(@"\t", "\t");

            return builder.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entry"></param>
        /// <returns></returns>
        protected override object FormatEntry(ISessionEntry entry)
        {
            if (null == entry)
                throw new ArgumentNullException("entry");

            if (entry.EntryList.Count == 0)
                return string.Empty;

            var builder = new StringBuilder(512 * (entry.EntryList.Count + 1));

            if (string.IsNullOrEmpty(this.Header) == false)
            {
                builder.Append(FormatterUtility.Format(entry.EntryList[0], this.Header));
            }

            for (var i = 0; i < entry.EntryList.Count; i++)
            {
                if (string.IsNullOrEmpty(this.Content))
                    builder.Append(entry.EntryList[i].ToText());
                else
                    builder.Append(FormatterUtility.Format(entry.EntryList[i], this.Content));
            }

            if (string.IsNullOrEmpty(this.Footer) == false)
            {
                builder.Append(FormatterUtility.Format(entry.EntryList[entry.EntryList.Count - 1], this.Footer));
            }

            builder.Replace(@"\r\n", "\r\n");
            return builder.ToString();
        }
    }
}
