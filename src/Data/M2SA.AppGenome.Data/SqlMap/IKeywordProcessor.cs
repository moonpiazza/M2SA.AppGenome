using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace M2SA.AppGenome.Data.SqlMap
{
    /// <summary>
    /// 
    /// </summary>
    interface IKeywordProcessor
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sqlText"></param>
        /// <param name="parameterValues"></param>
        /// <param name="escapeFunc"></param>
        KeywordProcessResult Process(string sqlText, IDictionary<string, object> parameterValues, Func<object, string> escapeFunc);
    }
}
