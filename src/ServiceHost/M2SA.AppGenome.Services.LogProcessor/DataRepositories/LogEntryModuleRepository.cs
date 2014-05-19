using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using M2SA.AppGenome.Data;
using M2SA.AppGenome.Reflection;

namespace M2SA.AppGenome.Services.LogProcessor.DataRepositories
{
    /// <summary>
    /// 
    /// </summary>
    public class LogEntryModuleRepository : SimpleRepositoryBase<LogEntryModule, long>, ILogEntryModuleRepository
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        protected override IDictionary<string, object> Convert(LogEntryModule entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            var parameterValues = entity.LogEntry.ToDictionary();
            parameterValues["AppId"] = entity.AppId;
            parameterValues["Detail"] = entity.LogEntry.ToText();
            return parameterValues;
        }

        public override bool Save(LogEntryModule model)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            var saved = base.Save(model);
            if (saved)
            {
                foreach (var labId in model.BizLabIds)
                {
                    this.SaveLogEntryLab(model.EntryId, labId);
                }
            }
            return saved;
        }

        bool SaveLogEntryLab(long entryId, long labId)
        {
            var sqlName = this.FormatSqlName("InsertLogEntryLab");
            var pValues = new Dictionary<string, object>(2);
            pValues.Add("EntryId", entryId);
            pValues.Add("LabId", labId);

            var rowsAffected = SqlHelper.ExecuteNonQuery(sqlName, pValues);
            var result = rowsAffected > 0;

            return result;
        }
    }
}
