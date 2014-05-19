using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using M2SA.AppGenome.Configuration;
using M2SA.AppGenome.Logging;
using M2SA.AppGenome.Logging.Listeners;
using M2SA.AppGenome.Services.LogProcessor.DataRepositories;

namespace M2SA.AppGenome.Services.LogProcessor
{
    /// <summary>
    /// 
    /// </summary>
    public class DatabaseListener : ListenerBase
    {
        private readonly static char LabSeparator = ',';
        
        /// <summary>
        /// 
        /// </summary>
        public IAppBaseProvider AppBaseProvider { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="config"></param>
        public override void Initialize(IConfigNode config)
        {
            base.Initialize(config);

            if (null == this.AppBaseProvider)
                this.AppBaseProvider = new AppBaseProvider();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entry"></param>
        public override void WriteMessage(ILogEntry entry)
        {
            if (null == this.AppBaseProvider)
                throw new ArgumentNullException("AppBaseProvider", "AppBaseProvider is not implemented.");

            if (entry == null)
                throw new ArgumentNullException("entry");

            var appId = this.AppBaseProvider.GetAppIdByName(entry.AppName);
            var labIds = SaveLabs(appId, entry);

            ResetNullProperties(entry);

            SaveStatisticItem(appId, labIds, entry);
            SaveEntry(appId, labIds, entry);
        }

        private static void ResetNullProperties(ILogEntry entry)
        {
            if (string.IsNullOrEmpty(entry.LogName))
                entry.LogName = string.Empty;
            if (string.IsNullOrEmpty(entry.ServerIP))
                entry.ServerIP = string.Empty;
            if (string.IsNullOrEmpty(entry.BizType))
                entry.BizType = string.Empty;
            if (string.IsNullOrEmpty(entry.URI))
                entry.URI = string.Empty;
        }

        private static void SaveEntry(string appId, IList<long> labIds, ILogEntry entry)
        {
            var item = new LogEntryModule();
            item.AppId = appId;
            item.BizLabIds = labIds;
            item.LogEntry = entry;
            item.Save();
        }

        private static void SaveStatisticItem(string appId, IList<long> labIds, ILogEntry entry)
        {
            var item = new LogStatisticItem();
            item.AppId = appId;
            item.LogName = entry.LogName;
            item.ServerIP = entry.ServerIP;
            item.BizType = entry.BizType;
            item.LogLevel = entry.LogLevel;
            item.URI = entry.URI;
            item.BizLabs = labIds;
            item.ResetStatisticTime(entry.LogTime);
            item.Save();
        }

        private static IList<long> SaveLabs(string appId, ILogEntry entry)
        {
            var labIds = new List<long>();
            if (false == string.IsNullOrEmpty(entry.BizLabs))
            {
                var labNames = entry.BizLabs;

                string[] labArray = labNames.Split(LabSeparator);

                foreach (string labName in labArray)
                {
                    if (string.IsNullOrWhiteSpace(labName)) continue;
                    var lab = new LogLab()
                    {
                        AppId = appId,
                        LabName = labName.Trim()
                    };
                    lab.Save();
                    if (false == labIds.Contains(lab.LabId))
                        labIds.Add(lab.LabId);
                }
            }

            return labIds;
        }
    }
}
