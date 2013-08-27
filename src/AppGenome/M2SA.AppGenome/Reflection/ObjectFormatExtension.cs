using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using M2SA.AppGenome.Reflection;

namespace M2SA.AppGenome.Reflection
{
    /// <summary>
    /// 
    /// </summary>
    public static class ObjectFormatExtension
    {
        static readonly Regex ParamRegex = new Regex(@"@(?<key>[A-Za-z_]\w+)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        static readonly Regex SnippetRegex = new Regex(@"\r?\n\s*?#(?<key>[A-Za-z_]\w+)\s*{(?<snippet>[^}]+)}", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        static readonly Regex SnippetHeaderRegex = new Regex(@"#(?<key>[A-Za-z_]\w+\$header)\s*{(?<item>[^}]+)}", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        static readonly Regex SnippetFooterRegex = new Regex(@"#(?<key>[A-Za-z_]\w+\$footer)\s*{(?<item>[^}]+)}", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        struct Snippet
        {
            public string Key { get; set; }
            public string Content { get; set; }
            public string Body { get; set; }
            public string Header { get; set; }
            public string HeaderBody { get; set; }
            public string Footer { get; set; }
            public string FooterBody { get; set; }
        }

        #region ToText(format)

        /// <summary>
        /// 
        /// </summary>
        /// <param name="val"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static string ToText(this object val, string format)
        {
            var result = ToText(val, format, ObjectExtension.ToText);
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="val"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static string ToXmlText(this object val, string format)
        {
            var result = ToText(val, format, ObjectExtension.ToXmlText);
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="val"></param>
        /// <param name="format"></param>
        /// <param name="ToTextFunc"></param>
        /// <returns></returns>
        public static string ToText(this object val, string format, Func<object, string> ToTextFunc)
        {
            var snippets = ParseSnippets(format);
            foreach (var pair in snippets)
            {
                format = format.Replace(pair.Value.Content, "");
                if (string.IsNullOrEmpty(pair.Value.Header) == false)
                    format = format.Replace(pair.Value.Header, "");
                if (string.IsNullOrEmpty(pair.Value.Footer) == false)
                    format = format.Replace(pair.Value.Footer, "");
            }

            var result = Format(val, format, snippets, ToTextFunc);

            return result;
        }

        static string Format(this object val, string format, IDictionary<string, Snippet> snippets, Func<object, string> ToTextFunc)
        {
            var result = format;
            var paramList = ParseTextToKeyList(format);
            var propertyValues = val.GetPropertyValues(paramList);
            foreach (var pair in propertyValues)
            {
                if (snippets.ContainsKey(pair.Key))
                    continue;

                var paramName = string.Format("@{0}", pair.Key);
                result = result.Replace(paramName, ToTextFunc(pair.Value));
            }

            foreach (var pair in propertyValues)
            {
                if (snippets.ContainsKey(pair.Key) == false)
                    continue;

                var snippet = snippets[pair.Key];
                var snippetOutput = string.Empty;
                if (null != pair.Value && pair.Value.GetType().IsListType())
                {
                    var list = (IList) pair.Value;
                    var sb = new StringBuilder(256);

                    if (string.IsNullOrEmpty(snippet.Header) == false)
                        sb.Append(Format(list, snippet.HeaderBody, snippets, ToTextFunc));

                    for(var i=0;i<list.Count;i++)
                    {
                        sb.Append(Format(list[i], snippet.Body, snippets, ToTextFunc));
                    }

                    if (string.IsNullOrEmpty(snippet.Footer) == false)
                        sb.Append(Format(list, snippet.FooterBody, snippets, ToTextFunc));
                    snippetOutput = sb.ToString();
                }
                
                var paramName = string.Format("@{0}", pair.Key);
                result = result.Replace(paramName, snippetOutput);
            }
            return result;
        }
        
        static IList<string> ParseTextToKeyList(string format)
        {
            var result = new List<string>();
            if (ParamRegex.IsMatch(format))
            {
                var matches = ParamRegex.Matches(format);
                foreach (Match match in matches)
                {
                    var paramName = match.Groups["key"].Value;
                    if (result.Contains(paramName) == false)
                    {
                        result.Add(paramName);
                    }
                }
            }

            return result;
        }

        static IDictionary<string, Snippet> ParseSnippets(string format)
        {
            var headerMap = MatchSnippetItem(format, SnippetHeaderRegex);
            var footerMap = MatchSnippetItem(format, SnippetFooterRegex);

            var snippetMap = new Dictionary<string, Snippet>();
            if (SnippetRegex.IsMatch(format))
            {
                var matches = SnippetRegex.Matches(format);
                foreach (Match match in matches)
                {
                    var key = match.Groups["key"].Value;
                    if (snippetMap.ContainsKey(key) == false)
                    {
                        var snippet = new Snippet()
                        {
                            Key = key, 
                            Body = match.Groups["snippet"].Value, 
                            Content = match.Value
                        };
                        var headerKey = string.Concat(key, "$header");
                        if (headerMap.ContainsKey(headerKey))
                        {
                            snippet.HeaderBody = headerMap[headerKey].Key;
                            snippet.Header = headerMap[headerKey].Value;
                        }

                        var footerKey = string.Concat(key, "$footer");
                        if (footerMap.ContainsKey(footerKey))
                        {
                            snippet.FooterBody = footerMap[footerKey].Key;
                            snippet.Footer = footerMap[footerKey].Value;
                        }
                        
                        snippetMap.Add(snippet.Key, snippet);
                    }
                }
            }

            return snippetMap;
        }

        static IDictionary<string, KeyValuePair<string, string>> MatchSnippetItem(string format, Regex regex)
        {
            var map = new Dictionary<string, KeyValuePair<string, string>>();
            if (regex.IsMatch(format))
            {
                var matches = regex.Matches(format);
                foreach (Match match in matches)
                {
                    var key = match.Groups["key"].Value;
                    if (map.ContainsKey(key) == false)
                    {
                        map.Add(key, new KeyValuePair<string, string>(match.Groups["item"].Value, match.Value));
                    }
                }
            }
            return map;
        }

        #endregion
    }
}
