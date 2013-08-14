using System;
using System.Collections.Generic;
using NUnit.Framework;
using M2SA.AppGenome.Data.SqlMap;

namespace M2SA.AppGenome.Data.Tests
{
	[TestFixture]
    public class SqlServerRepositoryTest : RepositoryTestBase
	{
        [TestFixtureSetUp]
        public override void Start()
        {
            base.Start();

            var databases = new List<DatabaseConfig>(1);
            var dbConfig = new DatabaseConfig
            {
                ConfigName = "TestDB",
                ConnectionString = "server=192.168.1.168;user id=sa;password=sa;database=Temp_Airport;",
                DBType = DatabaseType.SqlServer,
                ProviderName = "SqlServerProvider"
            };

            databases.Add(dbConfig);
            SqlMapping.AppendDatabases(databases);
        }

        [Test]
        public void DeleteAndSelectTest()
        {
            base.DeleteByIdTest();
        }
	}
}
