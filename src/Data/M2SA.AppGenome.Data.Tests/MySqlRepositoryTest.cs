using System;
using System.Collections.Generic;
using System.Transactions;
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
                ConnectionString = "server=127.0.0.1;port=3306;user id=root;password=db2test;database=testdb;",
                DBType = DatabaseType.MySql,
                ProviderName = "MySqlProvider"
            };

            databases.Add(dbConfig);
            SqlMapping.AppendDatabases(databases);
        }

        [Test]
        public void ScopeTest()
        {
            var repository = RepositoryManager.GetRepository<ITestRepository>();
            Assert.IsNotNull(repository);
            using (var scope = new TransactionScope())
            {
                var entity = new TestEntity() { Name = TestHelper.RandomizeString(), UpdateDate = DateTime.Now };
                Assert.AreEqual(0, entity.Id);
                var result = repository.Save(entity);
                Assert.IsTrue(result);
                Assert.IsTrue(0 < entity.Id);


                var entity2 = new TestEntity() { Name = TestHelper.RandomizeString(), UpdateDate = DateTime.Now };
                Assert.AreEqual(0, entity2.Id);
                var result2 = repository.Save(entity2);
                Assert.IsTrue(result2);
                Assert.IsTrue(0 < entity2.Id);
                scope.Complete();
            }
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

        [Test]
        public override void RaisErrorTest()
        {
            try
            {
                base.RaisErrorTest();
            }
            catch (Exception ex)
            {
                ex.Print();
            }
        }
	}
}