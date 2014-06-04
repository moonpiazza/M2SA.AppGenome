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
    public class InKeyProcessor : IKeywordProcessor
    {
        static readonly Regex InKeyRegex = new Regex(@"\s+in\s*\(\s*@(?<word>[A-Za-z_]\w+)\s*\)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        
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
            var macth = InKeyRegex.Match(sqlText);
            result.IsMatch = macth.Success;

            if (result.IsMatch)
            {
                var expression = macth.Value;
                var paramName = macth.Groups["word"].Value;

                var paramKey = FindParamKey(paramName, parameterValues);
                if (null == paramKey)
                    throw new ArgumentOutOfRangeException(paramName, string.Format("Must define param :{0}", paramName));

                var paramValue = parameterValues[paramKey];
                if (null == paramValue)
                    throw new ArgumentOutOfRangeException(paramName, string.Format("{0} is null ", paramName));

                var paramBuilder = new StringBuilder();
                if (paramValue is string)
                {
                    paramBuilder.Append(escapeFunc(paramValue));
                }
                else if (paramValue is IEnumerable)
                {
                    parameterValues.Remove(paramKey);

                    var list = (IEnumerable)paramValue;
                    var index = 0;
                    foreach (var item in list)
                    {
                        if (index == 0)
                        {
                            paramBuilder.Append(escapeFunc(item));
                            index++;
                        }
                        else
                        {
                            paramBuilder.Append(string.Concat(",", escapeFunc(item)));
                        }
                    }
                }
                else
                {
                    paramBuilder.Append(escapeFunc(paramValue));
                }

                var expressionResult = expression.Replace(string.Concat("@", paramName), paramBuilder.ToString());

                result.SqlExpression = sqlText.Replace(expression, expressionResult);
                result.ParameterValues = parameterValues;
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
