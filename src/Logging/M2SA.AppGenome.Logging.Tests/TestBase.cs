using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Reflection;
using NUnit.Framework;

namespace M2SA.AppGenome.Logging.Tests
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
