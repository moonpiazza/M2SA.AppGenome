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
        /// Sql业务模块的名称
        /// </summary>
        public string ModuleName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Namespace { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string FullName
        {
            get
            {
                return string.IsNullOrEmpty(this.Namespace)
                    ? this.ModuleName
                    : string.Concat(this.Namespace, SqlMapping.ModuleKeySeparator, this.ModuleName);
            }
        }

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
                Logging.LogManager.GetLogger(SqlHelper.SqlLogger)
                    .Warn("[SqlMap]{0}{1}{2}{3}{4} are covered.", this.Namespace, string.IsNullOrEmpty(this.Namespace) ? null:SqlMapping.ModuleKeySeparator
                        , this.ModuleName, SqlMapping.SqKeySeparator, sqlWrap.SqlName);
            this.SqlMap[sqlKey] = sqlWrap;
        }
    }
}
