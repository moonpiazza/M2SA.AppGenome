using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using NUnit.Framework;

namespace M2SA.AppGenome.Cache.Tests
{
    [TestFixture]
    public class ActiveCacheTest : TestBase
    {
        [Test]
        public void DefaultTest()
        {
            var memCache = (MemCached.MemCache)CacheManager.GetCache("activeCache");
            Assert.NotNull(memCache.LoadDataHandler);
            Assert.AreEqual(typeof(TestObjects.LoadMemoryItemHander), memCache.LoadDataHandler.GetType());

            var loadDataHandler = memCache.LoadDataHandler as TestObjects.LoadMemoryItemHander;

            var intervalSeconds = 3;
            var key = string.Concat("k-", TestHelper.RandomizeString());
            var val = loadDataHandler.LoadData(key);
            

            memCache.Set(key, val);

            Console.WriteLine("Now:{0}", DateTime.Now.ToString("HH:mm:ss.fff"));
            Thread.Sleep((int)memCache.ExpiryTime.TotalMilliseconds - intervalSeconds * 1000);

            var itemValue = memCache.Get(key);
            Assert.AreEqual(val, itemValue);

            Console.WriteLine("Now:{0}", DateTime.Now.ToString("HH:mm:ss.fff"));
            Thread.Sleep((intervalSeconds + 1) * 1000);

            itemValue = memCache.Get(key);
            Assert.AreEqual(val, itemValue);
        }
    }
}
