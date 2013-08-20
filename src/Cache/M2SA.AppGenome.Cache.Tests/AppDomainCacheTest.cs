using System;
using System.Threading;
using NUnit.Framework;
using M2SA.AppGenome.Reflection;

namespace M2SA.AppGenome.Cache.Tests
{
    [TestFixture]
    public class AppDomainCacheTest : CacheTestBase
    {
        protected override string CacheName 
        {
            get { return "AppDomainCache"; }
        }
    }
}
