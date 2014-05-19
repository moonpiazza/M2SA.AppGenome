using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using M2SA.AppGenome.Data;
using M2SA.AppGenome.Logging;
using M2SA.AppGenome.Services.LogProcessor.DataRepositories;

namespace M2SA.AppGenome.Services.LogProcessor
{
    /// <summary>
    /// 
    /// </summary>
    public class LogEntryModule : EntityBase<long>
    {
        #region Instance Properties

        /// <summary>
        /// 
        /// </summary>
        public long EntryId
        {
            get { return this.Id; }
            set { this.Id = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public string AppId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public IList<long> BizLabIds { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ILogEntry LogEntry { get; set; }

        #endregion //Instance Properties

        #region Persist Methods

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool Save()
        {
            var repository = RepositoryManager.GetRepository<ILogEntryModuleRepository>(ModuleEnvironment.ModuleName);
            var result = repository.Save(this);
            return result;
        }

        #endregion //Persist Methods
    }
}
