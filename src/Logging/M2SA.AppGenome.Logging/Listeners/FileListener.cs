using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using M2SA.AppGenome.Logging.Formatters;

namespace M2SA.AppGenome.Logging.Listeners
{
    /// <summary>
    /// 
    /// </summary>
    public class FileListener : ListenerBase
    {
        /// <summary>
        /// 
        /// </summary>
        public FilePattern FilePattern
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public TextFormatter Formatter
        {
            get;
            set;
        }
 
        /// <summary>
        /// 
        /// </summary>
        /// <param name="entry"></param>
        public override void WriteMessage(ILogEntry entry)
        {
            var filePath = this.FilePattern.GetFileName(entry);
            var message = this.Formatter.Format(entry);
            FileHelper.WriteInfo(filePath, message.ToString(), this.FilePattern.TryTimes);            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="config"></param>
        public override void Initialize(M2SA.AppGenome.Configuration.IConfigNode config)
        {
            base.Initialize(config);
            if (this.Formatter == null)
            {
                this.Formatter = new TextFormatter();
            }
        }
    }
}
