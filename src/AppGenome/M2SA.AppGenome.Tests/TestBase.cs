using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace M2SA.AppGenome.Tests
{    
    public class TestBase
    {
        [TestFixtureSetUp]
        public virtual void Start()
        {
            Console.WriteLine("ExtensibleApplication.GetInstance().BeginStart();");
            ExtensibleApplication.GetInstance().Start();
            ExtensibleApplication.GetInstance().Exit += new EventHandler(TestBase_OnExit);
        }

        void TestBase_OnExit(object sender, EventArgs e)
        {
            Console.WriteLine("ExtensibleApplication exit.");
        }

        [TestFixtureTearDown]
        public virtual void Stop()
        {
            Console.WriteLine("ExtensibleApplication.GetInstance().BeginStop();");
            ExtensibleApplication.GetInstance().Stop();
            Console.WriteLine("ExtensibleApplication.GetInstance().StopEnd();");
        }
    }
}
