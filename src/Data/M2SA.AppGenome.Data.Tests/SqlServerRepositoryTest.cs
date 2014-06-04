using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using M2SA.AppGenome.Data.Tests.Mocks;
using M2SA.AppGenome.Reflection;
using NUnit.Framework;
using M2SA.AppGenome.Reflection;
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
                ConnectionString = "server=127.0.0.1;user id=sa;password=db2Test;database=TestDb;",
                DBType = DatabaseType.SqlServer,
                ProviderName = "SqlServerProvider"
            };

            databases.Add(dbConfig);
            SqlMapping.AppendDatabases(databases);
        }

	    [Test]
        public void PaginationTest()
	    {
            base.LoadForPaginationTest();
	    }

        [Test]
        public override void FindByListTest()
        {
            base.FindByListTest();
        }
	}
}
