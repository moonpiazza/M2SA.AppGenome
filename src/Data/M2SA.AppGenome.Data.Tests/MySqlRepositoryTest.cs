using System;
using System.Collections.Generic;
using M2SA.AppGenome.Data.Tests.Mocks;
using M2SA.AppGenome.Reflection;
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
                ConnectionString = "server=127.0.0.1;port=3306;user id=root;password=db2Test;database=testdb;",
                DBType = DatabaseType.MySql,
                ProviderName = "MySqlProvider"
            };

            databases.Add(dbConfig);
            SqlMapping.AppendDatabases(databases);
        }


        [Test]
        public void PaginationTest()
        {
            base.LoadForPaginationTest();
        }
	}
}