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
    public class ObjectResolveTest : TestBase
    {
        [Test]
        public void ResolveServerGroupTest()
        {
            AppInstance.RegisterTypeAlias<ServerGroup>("ServerGroup");
            var targetType = TypeExtension.GetMapType("ServerGroup");
            Assert.IsNotNull(targetType);
            Assert.AreEqual(typeof(ServerGroup), targetType);

            var configXmlTemplete = @"<configuration xmlns:c='http://m2sa.net/Schema/Config'>
                    <serverGroup groupName='@groupName' c:type='serverGroup'>
                        <servers>
                          <server serverName='@serverName0' serverIP='@serverIP0' servicePort='@servicePort0'/>
                          <server serverName='@serverName1' serverIP='@serverIP1' servicePort='@servicePort1'/>
                        </servers>
                    </serverGroup></configuration>";

            var groupName = TestHelper.RandomizeString("group-");

            var serverName0 = TestHelper.RandomizeString("server-");            
            var serverIP0 = TestHelper.RandomizeString("ip-");
            var servicePort0 = TestHelper.RandomizeInt();

            var serverName1 = TestHelper.RandomizeString("server-");
            var serverIP1 = TestHelper.RandomizeString("ip-");
            var servicePort1 = TestHelper.RandomizeInt();

            var configInfo = configXmlTemplete.Replace("@groupName", groupName);
            configInfo = configInfo.Replace("@serverName0", serverName0).Replace("@serverIP0", serverIP0).Replace("@servicePort0", servicePort0.ToString());
            configInfo = configInfo.Replace("@serverName1", serverName1).Replace("@serverIP1", serverIP1).Replace("@servicePort1", servicePort1.ToString());

            var configXml = new XmlDocument();
            configXml.LoadXml(configInfo);            
            var node = configXml.SelectSingleNode("/configuration/serverGroup");

            var serverGroup = new ServerGroup();
            serverGroup.Initialize(new ConfigNode(node));

            Assert.AreEqual(groupName, serverGroup.GroupName);
            Assert.AreEqual(2, serverGroup.Servers.Count);

            Assert.AreEqual(serverName0, serverGroup.Servers[0].ServerName);
            Assert.AreEqual(serverIP0, serverGroup.Servers[0].ServerIP);
            Assert.AreEqual(servicePort0, serverGroup.Servers[0].ServicePort);

            Assert.AreEqual(serverName1, serverGroup.Servers[1].ServerName);
            Assert.AreEqual(serverIP1, serverGroup.Servers[1].ServerIP);
            Assert.AreEqual(servicePort1, serverGroup.Servers[1].ServicePort);

        }
    }
}
