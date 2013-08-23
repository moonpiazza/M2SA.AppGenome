using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Diagnostics;
using System.Threading;
using NUnit.Framework;
using M2SA.AppGenome.Diagnostics;
using M2SA.AppGenome.Threading;
using M2SA.AppGenome.Configuration;
using M2SA.AppGenome.Reflection;
using M2SA.AppGenome.Tests.TestObjects;

namespace M2SA.AppGenome.Tests
{
    [TestFixture]
    public class ObjectContainerTest : TestBase
    {
        [Test]
        public void ResolveInstanceTest()
        {
            var groupA = ObjectIOCFactory.ResolveInstance<ServerGroup>();
            var groupB = ObjectIOCFactory.GetSingleton<ServerGroup>();
            Assert.AreNotEqual(groupA, groupB);
        }

        /// <summary>
        /// 从Email.config创建EmailConfig的对象实例
        /// </summary>
        [Test]
        public void ResolveInstanceFromEmailConfigTest()
        {
            var emailConfig = ObjectIOCFactory.ResolveInstance<EmailConfig>();
            Assert.AreEqual("pop.m2sa.net", emailConfig.PopServer);

            Assert.AreEqual(110, emailConfig.PopServerPort);
            Assert.AreNotEqual(25, emailConfig.SmtpServerPort);
        }

        [Test]
        public void GetSingletonTest()
        {
            var poolA = ObjectIOCFactory.GetSingleton<SmartThreadPool>();
            var poolB = ObjectIOCFactory.GetSingleton<SmartThreadPool>();

            Assert.AreEqual(poolA, poolB);


            var processorA = ObjectIOCFactory.GetSingleton<TaskProcessor>();
            var processorB = ObjectIOCFactory.GetSingleton<TaskProcessor>();

            Assert.AreEqual(processorA, processorB);
        }


        /// <summary>
        /// /// <summary>
        /// 从domainSetting.config创建接口IDomainSetting的对象实例
        /// </summary>
        /// </summary>
        [Test]
        public void ResolveInstanceFromDomainConfigTest()
        {
            var domainSetting = ObjectIOCFactory.ResolveInstance<IDomainSetting>();

            Assert.NotNull(domainSetting);
            Assert.AreEqual(typeof(DomainSetting), domainSetting.GetType());
            Assert.AreEqual("utf-8", domainSetting.DefaultRule.Encode);


            var domainSetting2 = ObjectIOCFactory.ResolveInstance<IDomainSetting>();
            Assert.AreEqual(domainSetting, domainSetting2);

        }

        [Test]
        public void ResolveInstanceFromAliasTest()
        {
            AppInstance.RegisterTypeAlias<DomainSetting>("a");
            AppInstance.RegisterTypeAlias<EmailConfig>("a");

            var emailConfig = ObjectIOCFactory.ResolveInstance<EmailConfig>("a");
            Assert.NotNull(emailConfig);
            Assert.AreEqual(emailConfig.GetType(), typeof(EmailConfig));


            var domainSetting = ObjectIOCFactory.ResolveInstance<IDomainSetting>("a");
            Assert.NotNull(domainSetting);
            Assert.AreEqual(domainSetting.GetType(), typeof(DomainSetting));
            Assert.NotNull(domainSetting.DefaultRule);

            domainSetting.Print();

        }
    }
}
