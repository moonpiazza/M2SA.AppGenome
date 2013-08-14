using System;
using System.Collections.Generic;
using NUnit.Framework;
using M2SA.AppGenome.Data.SqlMap;

namespace M2SA.AppGenome.Data.Tests
{
	[TestFixture]
    public class MySqlRepositoryTest : RepositoryTestBase
	{
        [TestFixtureSetUp]
        public override void Start()
        {
            base.Start();

            var databases = new List<DatabaseConfig>(1);
            var dbConfig = new DatabaseConfig
            {
                ConfigName = "TestDB",
                ConnectionString = "server=127.0.0.1;port=3306;user id=root;password=AQ#ON|ibtDhb;database=testdb;",
                DBType = DatabaseType.MySql,
                ProviderName = "MySqlProvider"
            };

            databases.Add(dbConfig);
            SqlMapping.AppendDatabases(databases);
        } 
	}
}

/*
 
 <?xml version="1.0" encoding="utf-8" ?>
<configuration>
  <connectionStrings>
    <!--<add name="TestDB"  connectionString="server=127.0.0.1;port=3306;user id=root;password=AQ#ON|ibtDhb;database=testdb;"  providerName="MySqlProvider" />-->
    <add name="TestDB"  connectionString="server=192.168.1.168;user id=sa;password=sa;database=Temp_Airport;"  providerName="SqlServerProvider" />
  </connectionStrings>
 */
