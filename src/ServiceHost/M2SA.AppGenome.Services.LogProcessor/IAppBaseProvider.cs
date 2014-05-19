using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using M2SA.AppGenome.Data;

namespace M2SA.AppGenome.Services.LogProcessor
{
    /// <summary>
    /// 
    /// </summary>
    public interface IAppBaseProvider 
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="appName"></param>
        /// <returns></returns>
        string GetAppIdByName(string appName);
    }
}
