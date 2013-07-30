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

namespace M2SA.AppGenome.Tests
{
    [TestFixture]
    public class ConfigNodeTest : TestBase
    {
        [Test]
        public void ResolveTest()
        {
            var configXmlTemplete = @"<configuration xmlns:c='http://m2sa.net/Schema/Config'>
                <appbase debug='@isDebug'><appName>@appName</appName></appbase></configuration>";

            var appName = TestHelper.RandomizeString("app-");
            var isDebug = TestHelper.RandomizeBoolean();
            var configInfo = configXmlTemplete.Replace("@appName", appName).Replace("@isDebug", isDebug.ToString());

            var configXml = new XmlDocument();
            configXml.LoadXml(configInfo);            
            var node = configXml.SelectSingleNode("/configuration/appbase");
            var configNode = new ConfigNode(node);
            
            Assert.AreEqual(appName, configNode.GetProperty<string>("appName"));
            Assert.AreEqual(isDebug, configNode.GetProperty<bool>("debug"));            
        }

        [Test]
        public void ResolveComplexTypeTest()
        {
            var configXmlTemplete = @"<configuration xmlns:c='http://m2sa.net/Schema/Config'>
                    <appbase appName='ResolveComplexTypeTest'>
                        <typeAliases>
                          <typeAlias name='ServerGroup' c:type='M2SA.AppGenome.UnitTest.Mocks.ServerGroup, M2SA.AppGenome.UnitTest'/>
                        </typeAliases>
                    </appbase>
                    <serverGroup groupName='@groupName' c:type='serverGroup'>
                        <servers>
                          <server serverName='@serverName0' serverIP='@serverIP0' servicePort='@serverPort0'/>
                          <server serverName='@serverName1' serverIP='@serverIP1' servicePort='@serverPort1'/>
                        </servers>
                    </serverGroup>
                </configuration>";

            var groupName = TestHelper.RandomizeString("group-");
            var configInfo = configXmlTemplete.Replace("@groupName", groupName);

            var configXml = new XmlDocument();
            configXml.LoadXml(configInfo);
            var node = configXml.SelectSingleNode("/configuration/serverGroup");
            var configNode = new ConfigNode(node);

            Assert.AreEqual(groupName, configNode.GetProperty<string>("groupName"));
            Assert.AreEqual(2, configNode.GetNodeList("servers").Count);

        }

    }
}
