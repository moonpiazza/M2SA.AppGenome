using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading;
using NUnit.Framework;
using M2SA.AppGenome.Reflection;

namespace M2SA.AppGenome.Cache.Tests
{
    [TestFixture]
    public class SlaveServersTest : TestBase
    {
        [Test]
        public void CacheTest()
        {
            var cache = CacheManager.GetCache("slaveServers");
            var times = 49;      

            for (var i = 0; i < times; i++)
            {
                var key = string.Format("k-{0}-{1}", i, TestHelper.RandomizeString());
                var val = string.Concat("v-", TestHelper.RandomizeString());
                cache.Set(key, val);

                Thread.Sleep(50);
                var itemValue = cache.Get<string>(key);
                Assert.AreEqual(val, itemValue);

                Console.WriteLine("[{0}] {1} -> {2}", i, key, val);
            }
            
        }        
    }
}
