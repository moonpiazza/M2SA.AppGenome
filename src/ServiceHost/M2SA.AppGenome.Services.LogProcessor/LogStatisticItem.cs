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
    public class LogStatisticItem : EntityBase<long>
    {
        /// <summary>
        /// 
        /// </summary>
        public static readonly int NewTotal = 1;

        #region Instance Properties

        /// <summary>
        /// 
        /// </summary>
        public long ItemId
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
        public string LogName { get; set; }

        /// <summary>
        /// 会话标识
        /// </summary>
        public string ServerIP { get; set; }

        /// <summary>
        /// 业务类型
        /// </summary>
        public string BizType { get; set; }

        /// <summary>
        /// 日志级别
        /// </summary>
        public LogLevel LogLevel { get; set; }

        /// <summary>
        /// 日志源URI
        /// </summary>
        public string URI { get; set; }
            
        /// <summary>
        /// 
        /// </summary>
        public int Total { get; set; }        

        /// <summary>
        /// 
        /// </summary>
        public DateTime StatisticTime { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public IList<long> BizLabs { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public LogStatisticItem()
        {
            this.Total = NewTotal;
        }

        #endregion //Instance Properties

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logTime"></param>
        public void ResetStatisticTime(DateTime logTime)
        {
            var totalMinutes = logTime.Hour*60 + logTime.Minute;
            this.StatisticTime = logTime.Date.AddMinutes((totalMinutes / ModuleEnvironment.IntervalMinutes) * ModuleEnvironment.IntervalMinutes);
        }

        #region Persist Methods

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool Save()
        {
            var repository = RepositoryManager.GetRepository<ILogStatisticItemRepository>(ModuleEnvironment.ModuleName);
            var result = repository.Save(this);
            return result;
        }

        #endregion //Persist Methods

    }
}
