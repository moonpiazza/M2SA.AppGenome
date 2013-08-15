using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using M2SA.AppGenome.AppHub;
using M2SA.AppGenome.Tests.TestObjects;

namespace M2SA.AppGenome.Tests
{
    [TestFixture]
    public class AppHubTest
    {
        [Test]
        public void RunTest()
        {
            AppInstance.RegisterTypeAlias<TestApplication>("TestApplication");

            ObjectIOCFactory.GetSingleton<ApplicationHub>().Register<TestApplication>(new TestApplication() { Name = "TestApplicationForCode" });

            ApplicationHost.GetInstance().Init();

            ApplicationHost.GetInstance().Start();

            ApplicationHost.GetInstance().Stop();
        }
    }
}
