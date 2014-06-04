using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Common;

namespace M2SA.AppGenome.Data.SqlMap
{
    /// <summary>
    /// 
    /// </summary>
    public class SqlWrap
    {
        /// <summary>
        /// 数据库链接在配置中的名称
        /// </summary>
        public string DbName { get; set; }

        /// <summary>
        /// SQL标识
        /// </summary>
        public string SqlName { get; set; }

        /// <summary>
        /// SQL文本
        /// </summary>
        public string SqlText { get; set; }

        /// <summary>
        /// sql描述
        /// </summary>
        public string SqlDesc { get; set; }

        /// <summary>
        /// 分区方法名称
        /// </summary>
        public string PartitionName { get; set; }

        /// <summary>
        /// 表的主键列名称
        /// </summary>
        public string PrimaryKey { get; set; }

        /// <summary>
        /// SQL支持的数据库类型
        /// </summary>
        public DatabaseType SupportDbType { get; set; }

        /// <summary>
        /// SQL的类型
        /// </summary>
        public CommandType CommandType { get; set; }

        /// <summary>
        /// 等待命令执行的时间（以秒为单位）。默认值为 30 秒。 值 0 指示无限制，应尽量避免值 0，否则会无限期地等待执行命令。
        /// </summary>
        public int CommandTimeout { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public PaginationSql PaginationSql { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public SqlWrap()
        {
            this.CommandTimeout = 30;
            this.SupportDbType = DatabaseType.MySql;
            this.CommandType = CommandType.Text;
        }
    }
}
