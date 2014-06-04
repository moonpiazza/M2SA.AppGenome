using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace M2SA.AppGenome.Data.SqlMap
{
    /// <summary>
    /// 
    /// </summary>
    public class PaginationSql
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static PaginationSql Parse(SqlWrap sql)
        {
            ArgumentAssertion.IsNotNull(sql, "sql");

            if (null == sql.PaginationSql)
            {
                lock (sql)
                {
                    if (null == sql.PaginationSql)
                    {
                        var parser = new PaginationSqlParser(sql.SqlText);
                        sql.PaginationSql = parser.Parse();
                    }
                }
            }
            return sql.PaginationSql;
        }

        /// <summary>
        /// 
        /// </summary>
        public string PrimaryKey { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Columns { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Tables { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string WhereExpression { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string OrderExpression { get; set; }
    }
}
