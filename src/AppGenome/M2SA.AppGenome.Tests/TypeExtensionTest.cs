using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Xml;
using System.Diagnostics;
using System.Threading;
using NUnit.Framework;
using M2SA.AppGenome.Diagnostics;
using M2SA.AppGenome.Reflection;
using M2SA.AppGenome.Configuration;
using M2SA.AppGenome.Tests.TestObjects;

namespace M2SA.AppGenome.Tests
{
    [TestFixture]
    public class TypeExtensionTest : TestBase
    {
        [Test]
        public void RegisterTypeTest()
        {
            AppInstance.RegisterTypeAlias<ServerGroup>("ServerGroup");
            var actualType = TypeExtension.GetMapType("ServerGroup");

            Assert.IsNotNull(actualType);
            Assert.AreEqual(typeof(ServerGroup), actualType);
        }

        [Test]
        public void DefaultTypeMapTest()
        {
            Assert.AreEqual(typeof(ArrayList), typeof(IList).GetMapType());
            Assert.AreEqual(typeof(List<int>), typeof(IList<int>).GetMapType());

            Assert.AreEqual(typeof(Hashtable), typeof(Hashtable).GetMapType());
            Assert.AreEqual(typeof(Hashtable), typeof(IDictionary).GetMapType());

            Assert.AreEqual(typeof(Dictionary<string, int>), typeof(IDictionary<string, int>).GetMapType());            
        }
    }
}
