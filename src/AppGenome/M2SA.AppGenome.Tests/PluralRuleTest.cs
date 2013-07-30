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
using M2SA.AppGenome.Threading;
using M2SA.AppGenome.Configuration;

namespace M2SA.AppGenome.Tests
{
    [TestFixture]
    public class PluralRuleTest : TestBase
    {
        [Test]
        public void PluraTest()
        {
            Console.WriteLine(AppInstance.Config.Debug);

            Assert.AreEqual("items", "item".TranslateToPlural());
            Assert.AreEqual("servers", "server".TranslateToPlural());
            Assert.AreEqual("products", "product".TranslateToPlural());
            Assert.AreEqual("indexes", "index".TranslateToPlural());
            Assert.AreEqual("typeAliases", "typeAlias".TranslateToPlural());
            Assert.AreEqual("queues", "queue".TranslateToPlural());
            Assert.AreEqual("boxes", "box".TranslateToPlural());
            Assert.AreEqual("ases", "asis".TranslateToPlural());
            Assert.AreEqual("abuses", "abus".TranslateToPlural());
            Assert.AreEqual("sexes", "sex".TranslateToPlural());
            Assert.AreEqual("children", "child".TranslateToPlural());
            Assert.AreEqual("Listeners", "Listener".TranslateToPlural());
        }
    }
}
