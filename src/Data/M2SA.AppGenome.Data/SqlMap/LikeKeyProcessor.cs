using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace M2SA.AppGenome.Data.SqlMap
{
    /// <summary>
    /// 
    /// </summary>
    public class LikeKeyProcessor : IKeywordProcessor
    {
        static readonly Regex LikeKeyRegex = new Regex(@"\s+like\s+@(?<word>[A-Za-z_]\w+)[\s\)]", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sqlText"></param>
        /// <param name="parameterValues"></param>
        /// <param name="escapeFunc"></param>
        /// <returns></returns>
        public KeywordProcessResult Process(string sqlText, IDictionary<string, object> parameterValues, Func<object, string> escapeFunc)
        {
            ArgumentAssertion.IsNotNull(sqlText, "sqlText");
            ArgumentAssertion.IsNotNull(escapeFunc, "escapeFunc");

            var result = new KeywordProcessResult();
            result.SqlExpression = sqlText;
            result.ParameterValues = parameterValues;

            var macthes = LikeKeyRegex.Matches(sqlText);
            foreach (Match macth in macthes)
            {
                if (false == result.IsMatch) result.IsMatch = macth.Success;

                if (macth.Success)
                {
                    var paramName = macth.Groups["word"].Value;

                    var paramKey = FindParamKey(paramName, parameterValues);
                    if (null == paramKey)
                        throw new ArgumentOutOfRangeException(paramName, string.Format("Must define param :{0}", paramName));

                    var paramValue = parameterValues[paramKey];
                    if (null != paramValue)
                    {
                        var value = paramValue.ToString();
                        if (false == value.StartsWith("%") && false == value.EndsWith("%"))
                        {
                            value = string.Concat("%", value, "%");
                            parameterValues[paramKey] = value;
                        }
                    }
                }
            }

            return result;
        }

        private static string FindParamKey(string paramName, IDictionary<string, object> parameterValues)
        {
            if (parameterValues.ContainsKey(paramName))
                return paramName;

            var name = paramName.ToLower();
            foreach (var item in parameterValues)
            {
                if (item.Key.ToLower() == name)
                    return item.Key;
            }

            return null;
        }
    }
}
