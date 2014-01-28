using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using M2SA.AppGenome.Threading;

namespace M2SA.AppGenome.Data.SqlMap
{
    /// <summary>
    /// 
    /// </summary>
    public static class SqlMappingLoader
    {
        #region readonly info

        /// <summary>
        /// 配置数据库的数据库名称
        /// </summary>
        public static readonly string CoreDatabaseDBName = "Database.Core";

        /// <summary>
        /// SQLMapping在AppSettings中的Key
        /// </summary>
        public static readonly string SqlMappingFilePathKey = "SQLMapping.FilePath";

        /// <summary>
        /// SQLMapping在AppSettings中的Key
        /// </summary>
        public static readonly string SqlMappingFilePatternKey = "SQLMapping.FilePattern";

        /// <summary>
        /// 默认的SQL文件目录 "SqlMap"
        /// </summary>
        public static readonly string SqlMappingFilePath = "sqlmap";

        /// <summary>
        /// 默认的SQL文件格式 "*.sql.xml"
        /// </summary>
        public static readonly string SqlMappingFilePattern = "*.sql.xml";        

        #endregion

        static SqlMappingLoader()
        {
            var filePath = ConfigurationManager.AppSettings[SqlMappingFilePathKey];
            var filePattern = ConfigurationManager.AppSettings[SqlMappingFilePatternKey];

            if (string.IsNullOrEmpty(filePath) == false)
            {
                filePath = filePath.Replace(@"/", @"\");
                if (filePath.StartsWith(@"\"))
                {
                    filePath = filePath.Substring(1);
                }

                SqlMappingFilePath = filePath;
            }

            if (string.IsNullOrEmpty(filePattern) == false)
            {
                SqlMappingFilePattern = filePattern;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static void LoadSqlMapping()
        {
            LoadDatabases();

            LoadSqlModules();
        }

        #region LoadDatabases

        /// <summary>
        /// 加载数据库节点
        /// </summary>
        private static void LoadDatabases()
        {
            LoadDatabasesFromConnectionStrings();

            if (SqlMapping.ExistDatabase(CoreDatabaseDBName))
            {
                LoadDatabasesFromConfig();
            }
        }

        /// <summary>
        /// 加载配置节中的数据库节点
        /// </summary>
        private static void LoadDatabasesFromConnectionStrings()
        {
            var databases = new List<DatabaseConfig>(ConfigurationManager.ConnectionStrings.Count);

            foreach (ConnectionStringSettings connInfo in ConfigurationManager.ConnectionStrings)
            {
                var dbConfig = new DatabaseConfig
                {
                    ConfigName = connInfo.Name,
                    ConnectionString = connInfo.ConnectionString,
                    DBType = GetDatabaseTypeFromConfig(connInfo.Name, connInfo.ProviderName),
                    ProviderName = connInfo.ProviderName
                };
                databases.Add(dbConfig);
            }

            SqlMapping.AppendDatabases(databases);
        }

        /// <summary>
        /// 加载配置数据库中的数据库节点
        /// </summary>
        private static void LoadDatabasesFromConfig()
        {
            //var siteosDB = GetDatabase(SiteOSDBName);

            //var siteosDB = dic[SiteOSDBName.ToLower()];

            //var accesser = DataAccessFactory.CreateDataAccess(siteosDB.DBType, siteosDB.ConnectionString, null);


            //var table = accesser.ExecuteDataSet("SELECT * FROM SiteOS_Database").Tables[0];

            //if (table.Rows.Count > 0)
            //{
            //    for (var i = 0; i < table.Rows.Count; i++)
            //    {
            //        var dataRow = table.Rows[i];
            //        var configName = DataHelper.ToString(dataRow["ConfigName"]);

            //        var db = new DatabaseConfig {
            //            DBName = configName,
            //            ConnectionString = string.Format("server={0};database={1};User={2};Password={3};"
            //                                , DataHelper.ToString(dataRow["Server"]), DataHelper.ToString(dataRow["DatabaseName"])
            //                                , DataHelper.ToString(dataRow["Username"]), DataHelper.ToString(dataRow["Password"])),
            //            DBType = (DatabaseType)DataHelper.ToInt32(dataRow["DatabaseType"])
            //        };
            //        if (dic.ContainsKey(configName.ToLower()) == false)
            //        {
            //            dic.Add(db.DBName.ToLower(), db);
            //        }
            //        //2009-11-25以后检查加载更新时执行
            //        else
            //        {
            //            dic[db.DBName.ToLower()] = db;
            //        }
            //    }

            //}
        }

        private static DatabaseType GetDatabaseTypeFromConfig(string configName, string providerName)
        {
            var dbType = SqlMapping.DefaultDbType;
            string appSettingKey = string.Format("{0}.DatabaseType", configName);
            var dbTypeStr = ConfigurationManager.AppSettings[appSettingKey];

            if (string.IsNullOrEmpty(dbTypeStr))
            {
                if (providerName == typeof(MySql.MySqlProvider).Name)
                    dbType = DatabaseType.MySql;
                else if (providerName == typeof(SqlServer.SqlServerProvider).Name)
                    dbType = DatabaseType.SqlServer;
            }
            else
            {
                dbType = (DatabaseType)Enum.Parse(typeof(DatabaseType), dbTypeStr);
            }

            return dbType;
        }


        #endregion

        #region LoadSqlModules

        /// <summary>
        /// 加载SqlMap到内存中
        /// </summary>
        private static void LoadSqlModules()
        {
            var filePathInfo = SqlMappingFilePath.Split(new char[] { ',' });
            var filePatternInfo = SqlMappingFilePattern.Split(new char[] { ',' });
            foreach (var path in filePathInfo)
            {
                foreach (var pattern in filePatternInfo)
                {
                    LoadSqlModules(path, pattern);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="filePattern"></param>
        private static void LoadSqlModules(string filePath, string filePattern)
        {
            var moduleMap = new Dictionary<string, string>(4);
            var sqlWraps = new List<SqlWrap>(16);

            var basePath = AppDomain.CurrentDomain.RelativeSearchPath;
            if (string.IsNullOrEmpty(basePath))
            {
                basePath = AppDomain.CurrentDomain.BaseDirectory;
            }
            if (basePath.EndsWith(@"\") == false)
            {
                basePath = basePath + @"\";
            }

            var sqlFiles = Directory.GetFiles(string.Format("{0}{1}", basePath, filePath), filePattern, SearchOption.AllDirectories);
            foreach (var sqlFile in sqlFiles)
            {
                var sqlMapXml = new XmlDocument();
                sqlMapXml.Load(sqlFile);

                var nsmgr = new XmlNamespaceManager(sqlMapXml.NameTable);
                nsmgr.AddNamespace("sql", "http://m2sa.net/Schema/SqlMapping");

                var moduleNodes = sqlMapXml.DocumentElement.SelectNodes("sql:module", nsmgr);
                foreach (XmlNode moduleNode in moduleNodes)
                {
                    var moduleName = moduleNode.Attributes["moduleName"].InnerText;
                    var dbName = moduleNode.Attributes["dbName"].InnerText;

                    moduleMap[moduleName] = dbName;

                    var sqlMapElementes = moduleNode.SelectNodes("sql:sqlWrap", nsmgr);

                    foreach (XmlNode element in sqlMapElementes)
                    {
                        var sqlWrap = ConvertToSql(element, moduleName, dbName);
                        sqlWraps.Add(sqlWrap);
                    }
                }
            }

            SqlMapping.AppendModules(moduleMap);
            SqlMapping.AppendSqlWraps(sqlWraps);
        }

        private static SqlWrap ConvertToSql(XmlNode element, string moduleName, string moduleDBName)
        {
            var sqlWrap = new SqlWrap();
            sqlWrap.SQLName = string.Format("{0}.{1}", moduleName, element.Attributes["sqlName"].InnerText);
            sqlWrap.SQLText = element.InnerText;

            var node = element.Attributes.GetNamedItem("dbName");
            if (null != node)
            {
                sqlWrap.DBName = node.InnerText;
            }
            else
            {
                sqlWrap.DBName = moduleDBName;
            }

            node = element.Attributes.GetNamedItem("commandType");
            if (null != node)
            {
                sqlWrap.CommandType = (CommandType)Enum.Parse(typeof(CommandType), node.InnerText, true);
            }

            node = element.Attributes.GetNamedItem("timeout");
            if (null != node)
            {
                sqlWrap.CommandTimeout = Convert.ToInt32(node.InnerText);
            }

            node = element.Attributes.GetNamedItem("desc");
            if (null != node)
            {
                sqlWrap.SQLDesc = node.InnerText;
            }

            node = element.Attributes.GetNamedItem("partitionName");
            if (null != node)
            {
                sqlWrap.PartitionName = node.InnerText;
            }

            node = element.Attributes.GetNamedItem("supportDBType");
            if (null != node)
            {
                sqlWrap.SupportDBType = (DatabaseType)Enum.Parse(typeof(DatabaseType), node.InnerText);
            }

            return sqlWrap;
        }

        #endregion
    }
}
