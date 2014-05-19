using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using M2SA.AppGenome.Data;

namespace M2SA.AppGenome.Services.LogProcessor.DataRepositories
{
    /// <summary>
    /// 
    /// </summary>
    public class LogStatisticItemRepository : SimpleRepositoryBase<LogStatisticItem, long>, ILogStatisticItemRepository
    {
        public override bool Save(LogStatisticItem model)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            var saved = base.Save(model);
            if (saved)
            {
                foreach (var labId in model.BizLabs)
                {
                    this.SaveLogEntryLab(model.ItemId, labId);
                }
            }
            return saved;
        }

        bool SaveLogEntryLab(long itemId, long labId)
        {
            var sqlName = this.FormatSqlName("InsertLogStatisticItemLab");
            var pValues = new Dictionary<string, object>(3);
            pValues.Add("ItemId", itemId);
            pValues.Add("LabId", labId);
            pValues.Add("Total", LogStatisticItem.NewTotal);

            var rowsAffected = SqlHelper.ExecuteNonQuery(sqlName, pValues);
            var result = rowsAffected > 0;

            return result;
        }
    }
}
