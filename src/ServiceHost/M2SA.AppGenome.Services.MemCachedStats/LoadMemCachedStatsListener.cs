using System;
using System.Collections.Generic;
using System.Threading;
using System.Text;
using System.IO;
using M2SA.AppGenome.AppHub;
using M2SA.AppGenome.Cache;
using M2SA.AppGenome.Logging;
using M2SA.AppGenome.Threading;

namespace M2SA.AppGenome.Services.MemCachedStats
{
    /// <summary>
    /// 
    /// </summary>
    public class LoadMemCachedStatsListener : IExtensionApplication
    {
        /// <summary>
        /// 
        /// </summary>
        public string Cache { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public TimeSpan PocessInterval { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsRunning { get; private set; }

        #region IExtensionApplication 成员

        void IExtensionApplication.OnInit(ApplicationHost onwer, CommandArguments args)
        {
            //empty action
        }

        void IExtensionApplication.OnStart(ApplicationHost onwer, CommandArguments args)
        {
            this.IsRunning = true;
            Console.WriteLine("---------- Processor.OnStart... ----------");

            Process();
        }

        void IExtensionApplication.OnStop(ApplicationHost onwer, CommandArguments args)
        {
            Console.WriteLine("---------- Processor.OnStop... ----------");
            this.IsRunning = false;
        }

        #endregion


        void Process()
        {
            var loadStatsAction = new TimeAction(this.Cache, this.PocessInterval, LoadStats);
            AppInstance.GetTaskProcessor().RegisterAction(this.Cache, loadStatsAction);
        }

        private void LoadStats(DateTime time)
        {
            Console.WriteLine("Load MemCached State [{0}]", this.Cache);
            var memCache = CacheManager.GetCache(this.Cache);
            var cahceState = (IDictionary<string, IDictionary<string, string>>)memCache.GetState();

            Console.WriteLine("====================================================");
            Console.WriteLine("State[{0}] {1}", this.Cache, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
            foreach (var serverInfo in cahceState)
            {
                Console.WriteLine("Server:{0}", serverInfo.Key);
                foreach (var itemInfo in serverInfo.Value)
                {
                    Console.WriteLine("\t{0} : {1}", itemInfo.Key, itemInfo.Value);
                }
            }

            Console.WriteLine("====================================================");
            Console.WriteLine("");
        }
    }
}
