using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace M2SA.AppGenome.Data.SqlMap
{
    /// <summary>
    /// 
    /// </summary>
    public class PaginationSqlParser
    {
        private static readonly Regex ColumnRegex = new Regex(@"select\s+(?<w1>[\s\S]+?)\s+from\s+", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Multiline);
        private static readonly Regex OrderRegex = new Regex(@"\sorder\s+by(?<w1>[\s\S]+?)", RegexOptions.Compiled | RegexOptions.RightToLeft | RegexOptions.IgnoreCase | RegexOptions.Multiline);
        //private static readonly Regex GroupRegex = new Regex(@"\sgroup\s+by(?<w1>[\s\S]+?)", RegexOptions.Compiled | RegexOptions.RightToLeft | RegexOptions.IgnoreCase | RegexOptions.Multiline);
        private static readonly Regex WhereRegex = new Regex(@"\swhere\s+(?<w1>[\s\S]+?)", RegexOptions.Compiled | RegexOptions.RightToLeft | RegexOptions.IgnoreCase | RegexOptions.Multiline);
        //private static readonly Regex TableRegex = new Regex(@"select\s+(?<w1>[\s\S]+?)\s+from\s+", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Multiline);
        private string originalSql = "";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sql"></param>
        public PaginationSqlParser(string sql)
        {
            this.originalSql = sql;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public PaginationSql Parse()
        {
            var partialSql = this.originalSql.Trim();

            var paginationSql = new PaginationSql();
            var extractInfo = ExtractColumns(partialSql);
            paginationSql.Columns = extractInfo.Item1;
            partialSql = partialSql.Substring(partialSql.IndexOf(extractInfo.Item2) + extractInfo.Item2.Length);

            extractInfo = ExtractOrderExpression(partialSql);
            paginationSql.OrderExpression = extractInfo.Item1;
            if (false == string.IsNullOrEmpty(paginationSql.OrderExpression))
                partialSql = partialSql.Substring(0, partialSql.IndexOf(extractInfo.Item2));

            extractInfo = ExtractTables(partialSql);
            paginationSql.Tables = extractInfo.Item1;
            paginationSql.WhereExpression = extractInfo.Item2;

            return paginationSql;
        }

        static Tuple<string, string> ExtractColumns(string partialSql)
        {
            var result = string.Empty;
            var macth = ColumnRegex.Match(partialSql);
            if (macth.Success)
            {
                result = macth.Groups["w1"].Value.Trim();
            }

            return Tuple.Create<string, string>(result, macth.Value);
        }

        static Tuple<string, string> ExtractOrderExpression(string partialSql)
        {
            var result = string.Empty;
            var macth = OrderRegex.Match(partialSql);
            if (macth.Success)
            {
                result = macth.Groups["w1"].Value.Trim();
            }

            return Tuple.Create<string, string>(result, macth.Value);
        }

        static Tuple<string, string> ExtractTables(string partialSql)
        {
            var tables = string.Empty;
            var whereExpression = string.Empty;
            var whereMacth = WhereRegex.Match(partialSql);
            if (whereMacth.Success)
            {
                whereExpression = whereMacth.Groups["w1"].Value.Trim();
                tables = partialSql.Substring(0, partialSql.IndexOf(whereMacth.Value)).Trim();
            }
            else
            {
                tables = partialSql.Trim();
            }

            return Tuple.Create<string, string>(tables, whereExpression);
        }
    }
}
