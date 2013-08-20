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
    public class DefaultCacheTest : CacheTestBase
    {
        protected override string CacheName
        {
            get { return null; }
        }

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
    }
}
