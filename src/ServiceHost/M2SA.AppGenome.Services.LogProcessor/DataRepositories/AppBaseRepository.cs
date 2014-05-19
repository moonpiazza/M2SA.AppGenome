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
    public class AppBaseRepository : SimpleRepositoryBase<AppBase,int>, IAppBaseRepository
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="appName"></param>
        /// <returns></returns>
        public AppBase FindByName(string appName)
        {
            var sqlName = this.FormatSqlName("SelectByName");
            var sqlParams = new Dictionary<string, object>(1);
            sqlParams.Add("AppName", appName);

            var dataset = SqlHelper.ExecuteDataSet(sqlName, sqlParams);

            AppBase model = null;
            if (dataset.Tables[0].Rows.Count == 1)
            {
                model = this.Convert(dataset.Tables[0].Rows[0]);
            }

            return model;
        }
    }
}
