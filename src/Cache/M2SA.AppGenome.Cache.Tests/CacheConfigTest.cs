using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using NUnit.Framework;
using M2SA.AppGenome.Reflection;

namespace M2SA.AppGenome.Cache.Tests
{
    [TestFixture]
    public class CacheConfigTest : TestBase
    {
        [Test]
        public void DefaultTest()
        {
            var cache = CacheManager.GetCache();
            var dCache = CacheManager.GetCache("default");
            Assert.IsNotNull(dCache);
            Assert.AreEqual(cache, dCache);

            cache.Print();
        }

        [Test]
        public void MultiServersTest()
        {
            var cache = CacheManager.GetCache("multiServers");
            var mCache = (MemCached.MemCache)cache;
            Assert.IsNotNull(mCache);

            Assert.AreEqual(mCache.ExpiryTime, TimeSpan.Zero);
            Assert.AreEqual(mCache.Servers.Count, 2);

            mCache.Print();
        }

        [Test]
        public void SlaveServersTest()
        {
            var cache = CacheManager.GetCache("slaveServers");
            var mCache = (MemCached.MemCache)cache;
            Assert.IsNotNull(mCache);

            Assert.AreEqual(mCache.ExpiryTime, TimeSpan.Zero);
            Assert.AreEqual(mCache.Servers.Count, 2);

            mCache.Print();
        }

        [Test]
        public void ActiveCacheTest()
        {
            var cache = CacheManager.GetCache("activeCache");
            var mCache = (MemCached.MemCache)cache;
            Assert.IsNotNull(mCache);
            Assert.IsNotNull(mCache.Notify);
            Assert.IsNotNull(mCache.LoadDataHandler);

            mCache.Print();
        }
    }
}
