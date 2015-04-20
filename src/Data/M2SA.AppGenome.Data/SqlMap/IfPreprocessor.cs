using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using M2SA.AppGenome.Logging;
using M2SA.AppGenome.Reflection;

namespace M2SA.AppGenome.Data.SqlMap
{
    /// <summary>
    /// 
    /// </summary>
    public class IfPreprocessor : IKeywordProcessor
    {
        static readonly Regex InKeyRegex = new Regex(@"\s+#if\s*\(\s*@(?<param>[A-Za-z_]\w+)\s*\)\s+\{(?<statement>[^}]+)\}", RegexOptions.IgnoreCase | RegexOptions.Compiled);

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

            ProcessByKeyRegex(InKeyRegex, sqlText, parameterValues, result);
            return result;
        }

        private static void ProcessByKeyRegex(Regex keyRegex, string sqlText, IDictionary<string, object> parameterValues, KeywordProcessResult result)
        {
            var macthes = keyRegex.Matches(sqlText);
            var processedParams = new List<string>(macthes.Count);
            foreach (Match macth in macthes)
            {
                if (false == result.IsMatch) result.IsMatch = macth.Success;

                if (macth.Success)
                {
                    var expression = macth.Value;
                    var paramName = macth.Groups["param"].Value;
                    var statement = macth.Groups["statement"].Value;

                    var paramKey = FindParamKey(paramName, parameterValues);
                    if (processedParams.Contains(paramName))
                        continue;
                    else if (null == paramKey)
                        throw new ArgumentOutOfRangeException(paramName, string.Format("Must define param :{0}", paramName));

                    var paramValue = parameterValues[paramKey];
                    var expressionResult = null == paramValue ? "" : statement;
                    result.SqlExpression = result.SqlExpression.Replace(expression, expressionResult);
                }
            }
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
