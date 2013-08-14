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
            ExtensibleApplication.GetInstance().Start();
            AppInstance.RegisterTypeAlias<RepositoryFactory>("RepositoryFactory");
        }

        [TestFixtureTearDown]
        public virtual void Stop()
        {
            ExtensibleApplication.GetInstance().Stop();
            stopwatch.Stop();
            Console.WriteLine("Application run : {0}", stopwatch.Elapsed);
        }
    }
}
