using System;
using NUnit.Framework;
using M2SA.AppGenome.Data.Tests.Mocks;

namespace M2SA.AppGenome.Data.Tests
{
	[TestFixture]
	public class RepositoryTest : TestBase
	{
		[Test]
		public void InsertTest()
		{
            var repository = RepositoryManager.GetRepository<ITestRepository>();
            Assert.IsNotNull(repository);

            var entity = new TestEntity() { Name = TestHelper.RandomizeString(), UpdateDate = DateTime.Now };
            Assert.AreEqual(0, entity.Id);
            var result = repository.Save(entity);
            Assert.IsTrue(result);
            Assert.IsTrue(0 < entity.Id);            
		}

        [Test]
        public void UpdateTest()
        {
            var repository = RepositoryManager.GetRepository<ITestRepository>();
            Assert.IsNotNull(repository);

            var entity = new TestEntity() { Name = TestHelper.RandomizeString(), UpdateDate = DateTime.Now };
            Assert.AreEqual(0, entity.Id);
            var result = repository.Save(entity);
            Assert.IsTrue(result);
            Assert.IsTrue(0 < entity.Id);  

            result = repository.Save(entity);
            Assert.IsTrue(result);
        }

        [Test]
        public void SelectByIdTest()
        {
            var repository = RepositoryManager.GetRepository<ITestRepository>();
            Assert.IsNotNull(repository);

            var entity = new TestEntity() { Name = TestHelper.RandomizeString(), UpdateDate = DateTime.Now };
            Assert.AreEqual(0, entity.Id);
            var result = repository.Save(entity);
            Assert.IsTrue(result);
            Assert.IsTrue(0 < entity.Id);

            var dbEntity = repository.FindById(entity.Id);
            Assert.AreEqual(entity.Id, dbEntity.Id);
            Assert.AreEqual(entity.Name, dbEntity.Name);
            Assert.AreEqual(0, (entity.UpdateDate - dbEntity.UpdateDate).Minutes);
        }

        [Test]
        public void DeleteByIdTest()
        {
            var repository = RepositoryManager.GetRepository<ITestRepository>();
            Assert.IsNotNull(repository);

            var entity = new TestEntity() { Name = TestHelper.RandomizeString(), UpdateDate = DateTime.Now };
            Assert.AreEqual(0, entity.Id);
            var result = repository.Save(entity);
            Assert.IsTrue(result);
            Assert.IsTrue(0 < entity.Id);

            var dbEntity = repository.FindById(entity.Id);
            Assert.AreEqual(entity.Id, dbEntity.Id);

            result = repository.Remove(dbEntity);
            Assert.IsTrue(result);
            Assert.AreEqual(PersistentState.Deleted, dbEntity.PersistentState);
            Console.WriteLine(dbEntity.Id);
            dbEntity = repository.FindById(entity.Id);
            Assert.IsNull(dbEntity);
        }
        [Test]
        public void SelectAllTest()
        {
            var repository = RepositoryManager.GetRepository<ITestRepository>();
            Assert.IsNotNull(repository);

            var entity = new TestEntity() { Name = TestHelper.RandomizeString(), UpdateDate = DateTime.Now };
            Assert.AreEqual(0, entity.Id);
            var result = repository.Save(entity);
            Assert.IsTrue(result);
            Assert.IsTrue(0 < entity.Id);

            var entities = repository.LoadAll();
            Assert.IsNotNull(entities);
            Assert.IsTrue(0 < entities.Count);            
        }
	}
}
