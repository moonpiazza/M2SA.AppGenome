using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Linq;
using NUnit.Framework;
using M2SA.AppGenome.Reflection;

namespace M2SA.AppGenome.Cache.Tests
{
    [TestFixture]
    public class DefaultCacheTest : TestBase
    {
        [Test]
        public void DefaultTest()
        {
            var key = string.Concat("k-", TestHelper.RandomizeString());
            var val = string.Concat("v-", TestHelper.RandomizeString());
            var cache = CacheManager.GetCache();
            cache.Set(key, val);

            var dCache = CacheManager.GetCache("default");
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
            var memCache = (MemCached.MemCache)CacheManager.GetCache();
            memCache.Set(key, val);            

            Console.WriteLine("Now:{0}", DateTime.Now.ToString("HH:mm:ss.fff"));
            Thread.Sleep((int)memCache.ExpiryTime.TotalMilliseconds - intervalSeconds*1000);

            var itemValue = memCache.Get(key);
            Assert.AreEqual(val, itemValue);

            Console.WriteLine("Now:{0}", DateTime.Now.ToString("HH:mm:ss.fff"));
            Thread.Sleep((intervalSeconds+1)*1000);

            itemValue = memCache.Get(key);
            Assert.IsNull(itemValue);
        }

        [Test]
        public void MemStateTest()
        {
            var memCache = CacheManager.GetCache();

            var cahceState = (IDictionary<string, IDictionary<string, string>>)memCache.GetState();


            cahceState.Print();
        }
    }
}
