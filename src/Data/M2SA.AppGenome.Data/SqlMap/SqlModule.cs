using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace M2SA.AppGenome.Data.SqlMap
{
    /// <summary>
    /// 
    /// </summary>
    public class SqlModule
    {
        private string dbName = null;

        /// <summary>
        /// 数据库链接在配置中的名称
        /// </summary>
        public string ModuleName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Namespace { get; set; }

        /// <summary>
        /// 数据库链接在配置中的名称
        /// </summary>
        public string DbName 
        {
            get 
            {
                if (string.IsNullOrEmpty(this.dbName))
                {
                    return this.Namespace;
                }
                return this.dbName;
            }
            set { this.dbName = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public IDictionary<string, SqlWrap> SqlMap { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public SqlModule()
        {
            this.SqlMap = new Dictionary<string, SqlWrap>();
        }

        /// <summary>
        /// 
        /// </summary>
        public void AddSqlWrap(SqlWrap sqlWrap)
        {
            var sqlKey = sqlWrap.SqlName.ToLower();
            if (this.SqlMap.ContainsKey(sqlKey))
                Logging.LogManager.GetLogger(SqlHelper.SqlLogger).Warn("[SqlMap]{0} are covered.", sqlWrap.SqlName);
            this.SqlMap[sqlKey] = sqlWrap;
        }
    }
}
