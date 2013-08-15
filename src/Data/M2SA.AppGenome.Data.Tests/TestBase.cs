using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Reflection;
using NUnit.Framework;
using M2SA.AppGenome.Data.Tests.Mocks;

namespace M2SA.AppGenome.Data.Tests
{
    [TestFixture]
    public abstract class TestBase
    {
        Stopwatch stopwatch = null;
        
        [TestFixtureSetUp]
        public virtual void Start()
        {
            stopwatch = Stopwatch.StartNew();
            ApplicationHost.GetInstance().Start();
            AppInstance.RegisterTypeAlias<RepositoryFactory>("RepositoryFactory");
        }

        [TestFixtureTearDown]
        public virtual void Stop()
        {
            ApplicationHost.GetInstance().Stop();
            stopwatch.Stop();
            Console.WriteLine("ApplicationHost run : {0}", stopwatch.Elapsed);
        }
    }
}
