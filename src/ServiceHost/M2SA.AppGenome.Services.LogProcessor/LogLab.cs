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
    public class LogLab : EntityBase<long>
    {
        #region Instance Properties

        /// <summary>
        /// 
        /// </summary>
        public long LabId
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
        public string LabName { get; set; }

        #endregion //Instance Properties

        #region Persist Methods

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool Save()
        {
            var repository = RepositoryManager.GetRepository<ILogLabRepository>(ModuleEnvironment.ModuleName);
            var result = repository.Save(this);
            return result;
        }

        #endregion //Persist Methods
    }
}
