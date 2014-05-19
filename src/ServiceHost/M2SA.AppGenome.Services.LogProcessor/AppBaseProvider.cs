using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using M2SA.AppGenome.Data;
using M2SA.AppGenome.Services.LogProcessor.DataRepositories;

namespace M2SA.AppGenome.Services.LogProcessor
{
    /// <summary>
    /// 
    /// </summary>
    public class AppBaseProvider : IAppBaseProvider
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="appName"></param>
        /// <returns></returns>
        public string GetAppIdByName(string appName)
        {
            var repository = RepositoryManager.GetRepository<IAppBaseRepository>(ModuleEnvironment.ModuleName);
            var appBase = repository.FindByName(appName);

            var appId = null == appBase ? appName : appBase.AppId.ToString();
            return appId;
        }
    }

       /// <summary>
    /// 
    /// </summary>
    public class AppBase : EntityBase<int>
    {
        /// <summary>
        /// 
        /// </summary>
        public int AppId
        {
            get { return this.Id; }
            set { this.Id = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public string AppName { get; set; }
    }
}
