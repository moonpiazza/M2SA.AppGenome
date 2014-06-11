using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using M2SA.AppGenome.Reflection;

namespace M2SA.AppGenome.Logging.Formatters
{
    /// <summary>
    /// 
    /// </summary>
    public class FormatterUtility
    {
        static readonly Regex ParamRegex = new Regex(@"@(?<word>[A-Za-z_]\w+)", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static List<string> ParseTextToKeyList(string text)
        {
            var result = new List<string>();
            if (ParamRegex.IsMatch(text))
            {
                var matches = ParamRegex.Matches(text);
                foreach (Match match in matches)
                {
                    var paramName = match.Groups["word"].Value;
                    if (result.Contains(paramName) == false)
                    {
                        result.Add(paramName);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entry"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string Format(ILogEntry entry, string text)
        {
            var result = text;

            var propertyValues = ToMap(entry, text);
            var propKeys = propertyValues.Keys.ToList();
            propKeys.Sort();
            for (var i = propKeys.Count-1; i >= 0; i--)
            {
                var paramName = string.Format("@{0}", propKeys[i]);
                result = result.Replace(paramName, propertyValues[propKeys[i]].ToString());  
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entry"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public static IDictionary<string, object> ToMap(ILogEntry entry, string text)
        {
            IDictionary<string, object> result = null;
            if (string.IsNullOrEmpty(text))
            {
                result = entry.GetPropertyValues();
            }
            else
            {
                var paramList = ParseTextToKeyList(text);
                result = new Dictionary<string, object>(paramList.Count);

                var propertyValues = entry.GetPropertyValues(paramList);
                paramList.ForEach(param => {
                    object val = null;
                    if (propertyValues.ContainsKey(param))
                        val = propertyValues[param];
                    if (entry.ExtendInfo.ContainsKey(param))
                        val = entry.ExtendInfo[param];

                    if (null == val)
                        val = string.Empty;
                    else if (val.GetType().IsPrimitiveType() == false)
                        val = Format(val);
                    result.Add(param, val);
                });
            }

            return result;
        }

        static string Format(object obj)
        {
            return obj.ToText();
        }
    }
}
