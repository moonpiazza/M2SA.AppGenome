using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace M2SA.AppGenome.Cache.Tests
{
    public abstract class TestBase
    {
        [TestFixtureSetUp]
        public virtual void Start()
        {
            ApplicationHost.GetInstance().Start();
            AppInstance.RegisterTypeAliasByModule<TestObjects.LoadMemoryItemHander>(AppConfig.CachedKey);
        }

        [TestFixtureTearDown]
        public virtual void Stop()
        {
            ApplicationHost.GetInstance().Stop();
        }
    }
}
