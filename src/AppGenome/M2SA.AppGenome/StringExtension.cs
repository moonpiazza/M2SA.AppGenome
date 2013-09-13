using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace M2SA.AppGenome
{
    /// <summary>
    /// 
    /// </summary>
    public static class StringExtension
    {
        private static readonly Regex IntegerRegex = new Regex(@"^\d{1,19}$", RegexOptions.Compiled);
        private static readonly Regex EMailRegex = new Regex(@"^[\w-]+(\.[\w-]+)*(\+[\w-]+)?@[\w-]+(\.[\w-]+)+$", RegexOptions.Compiled);
        private static readonly Regex HttpUrlRegex = new Regex(@"^(http|https)\://([a-zA-Z0-9\.\-]+(\:[a-zA-Z0-9\.&amp;%\$\-]+)*@)?((25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9])\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9]|0)\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9]|0)\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[0-9])|([a-zA-Z0-9\-]+\.)*[a-zA-Z0-9\-]+\.[a-zA-Z]{2,4})(\:[0-9]+)?(/[^/][a-zA-Z0-9\.\,\?\'\\/\+&amp;%\$#\=~_\-@]*)*$", RegexOptions.IgnoreCase & RegexOptions.Compiled);

        private static bool IsMatchRegex(string input, Regex regex)
        {
            var result = !string.IsNullOrEmpty(input);
            if (result)
                result = regex.IsMatch(input);
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsMatchInteger(this string input)
        {
            return IsMatchRegex(input, IntegerRegex);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsMatchEMail(this string input)
        {
            return IsMatchRegex(input, EMailRegex);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsMatchHttpUrl(this string input)
        {
            return IsMatchRegex(input, HttpUrlRegex);
        }
    }
}
