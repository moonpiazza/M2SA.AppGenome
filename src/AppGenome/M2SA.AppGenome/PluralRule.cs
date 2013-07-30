using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace M2SA.AppGenome
{
    /// <summary>
    /// 单复数规则
    /// <remarks>rails 单复数规则实现</remarks>
    /// </summary>
    public static class PluralRule
    {
        #region rules

        static readonly IList<KeyValuePair<Regex, string>> PluralRegexes = null;

        static PluralRule()
        {
            PluralRegexes = new List<KeyValuePair<Regex, string>>(10);
            PluralRegexes.Add(new KeyValuePair<Regex, string>(
              new Regex("$", RegexOptions.IgnoreCase | RegexOptions.Compiled), @"s"));
            PluralRegexes.Add(new KeyValuePair<Regex, string>(
              new Regex("s$", RegexOptions.IgnoreCase | RegexOptions.Compiled), @"s"));
            PluralRegexes.Add(new KeyValuePair<Regex, string>(
              new Regex("(ax|test)is$", RegexOptions.IgnoreCase | RegexOptions.Compiled), @"$1es"));
            PluralRegexes.Add(new KeyValuePair<Regex, string>(
              new Regex("(octop|vir)us$", RegexOptions.IgnoreCase | RegexOptions.Compiled), @"$1i"));
            PluralRegexes.Add(new KeyValuePair<Regex, string>(
              new Regex("(alias|status)$", RegexOptions.IgnoreCase | RegexOptions.Compiled), @"$1es"));
            PluralRegexes.Add(new KeyValuePair<Regex, string>(
              new Regex("(bu)s$", RegexOptions.IgnoreCase | RegexOptions.Compiled), @"$1ses"));
            PluralRegexes.Add(new KeyValuePair<Regex, string>(
              new Regex("(buffal|tomat)o$", RegexOptions.IgnoreCase | RegexOptions.Compiled), @"$1oes"));
            PluralRegexes.Add(new KeyValuePair<Regex, string>(
              new Regex("([ti])um$", RegexOptions.IgnoreCase | RegexOptions.Compiled), @"$1a"));
            PluralRegexes.Add(new KeyValuePair<Regex, string>(
              new Regex("sis$", RegexOptions.IgnoreCase | RegexOptions.Compiled), @"ses"));
            PluralRegexes.Add(new KeyValuePair<Regex, string>(
              new Regex("(?:([^f])fe|([lr])f)$", RegexOptions.IgnoreCase | RegexOptions.Compiled), @"$1$2ves"));
            PluralRegexes.Add(new KeyValuePair<Regex, string>(
              new Regex("(hive)$", RegexOptions.IgnoreCase | RegexOptions.Compiled), @"$1s"));
            PluralRegexes.Add(new KeyValuePair<Regex, string>(
              new Regex("([^aeiouy]|qu)y$", RegexOptions.IgnoreCase | RegexOptions.Compiled), @"$1ies"));
            PluralRegexes.Add(new KeyValuePair<Regex, string>(
              new Regex("(x|ch|ss|sh)$", RegexOptions.IgnoreCase | RegexOptions.Compiled), @"$1es"));
            PluralRegexes.Add(new KeyValuePair<Regex, string>(
              new Regex("(matr|vert|ind)ix|ex$", RegexOptions.IgnoreCase | RegexOptions.Compiled), @"$1ices"));
            PluralRegexes.Add(new KeyValuePair<Regex, string>(
              new Regex("([m|l])ouse$", RegexOptions.IgnoreCase | RegexOptions.Compiled), @"$1ice"));
            PluralRegexes.Add(new KeyValuePair<Regex, string>(
              new Regex("^(ox)$", RegexOptions.IgnoreCase | RegexOptions.Compiled), @"$1en"));
            PluralRegexes.Add(new KeyValuePair<Regex, string>(
              new Regex("(quiz)$", RegexOptions.IgnoreCase | RegexOptions.Compiled), @"$1zes"));

            PluralRegexes.Add(new KeyValuePair<Regex, string>(
              new Regex("index$", RegexOptions.IgnoreCase | RegexOptions.Compiled), @"indexes"));

            PluralRegexes.Add(new KeyValuePair<Regex, string>(
              new Regex("^person$", RegexOptions.IgnoreCase | RegexOptions.Compiled), @"people"));
            PluralRegexes.Add(new KeyValuePair<Regex, string>(
              new Regex("^man$", RegexOptions.IgnoreCase | RegexOptions.Compiled), @"men"));
            PluralRegexes.Add(new KeyValuePair<Regex, string>(
              new Regex("^child$", RegexOptions.IgnoreCase | RegexOptions.Compiled), @"children"));
            PluralRegexes.Add(new KeyValuePair<Regex, string>(
              new Regex("^sex$", RegexOptions.IgnoreCase | RegexOptions.Compiled), @"sexes"));
            PluralRegexes.Add(new KeyValuePair<Regex, string>(
              new Regex("^move$", RegexOptions.IgnoreCase | RegexOptions.Compiled), @"moves"));
        }

        #endregion

        /// <summary>
        /// 获得单词的复数形式
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string TranslateToPlural(this string name)
        {
            var result = name;
            for (var i = PluralRegexes.Count - 1; i >= 0; i--)
            { 
                var regex = PluralRegexes[i].Key;                
                if (regex.IsMatch(name))
                {
                    result = PluralRegexes[i].Key.Replace(name, PluralRegexes[i].Value);
                    break;
                }                
            }

            return result;
        }
    }
}
