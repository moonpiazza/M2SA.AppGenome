using System;
using System.Threading;
using NUnit.Framework;
using M2SA.AppGenome.Reflection;

namespace M2SA.AppGenome.Cache.Tests
{
    public abstract class CacheTestBase : TestBase
    {
        protected abstract string CacheName { get; }

        [Test]
        public void GetTest()
        {
            var key = string.Concat("k-", TestHelper.RandomizeString());
            var val = string.Concat("v-", TestHelper.RandomizeString());
            var cache = CacheManager.GetCache(CacheName);
            cache.Set(key, val);

            var dCache = CacheManager.GetCache(CacheName);
            var itemValue = dCache.Get(key);
            Assert.AreEqual(val, itemValue);
            Assert.AreEqual(cache, dCache);
        }

        [Test]
        public void ExpiryTimeTest()
        {
            var intervalSeconds = 3;
            var key = string.Concat("k-", TestHelper.RandomizeString());
            var val = string.Concat("v-", TestHelper.RandomizeString());
            var cache = CacheManager.GetCache(CacheName);
            cache.Set(key, val);

            Console.WriteLine("Now:{0}", DateTime.Now.ToString("HH:mm:ss.fff"));
            Thread.Sleep((int)cache.ExpiryTime.TotalMilliseconds - intervalSeconds * 1000);

            var itemValue = cache.Get(key);
            Assert.AreEqual(val, itemValue);

            Console.WriteLine("Now:{0}", DateTime.Now.ToString("HH:mm:ss.fff"));
            Thread.Sleep((intervalSeconds + 1) * 1000);

            itemValue = cache.Get(key);
            Assert.IsNull(itemValue);
        }

        [Test]
        public void MemStateTest()
        {
            var memCache = CacheManager.GetCache(CacheName);

            var cahceState = memCache.GetState();

            cahceState.Print();
        }
    }
}
