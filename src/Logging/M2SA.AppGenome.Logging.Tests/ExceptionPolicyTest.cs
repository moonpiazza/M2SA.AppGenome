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

namespace M2SA.AppGenome.Logging.Tests
{
    /// <summary>
    /// 
    /// </summary>
    /// <include file='appgenome.config' />
    [TestFixture]
    public class ExceptionPolicyTest : TestBase
    {
        [Test]
        public void TestIsListType()
        {
            try
            {
                int.Parse("a");
            }
            catch (Exception ex)
            {
                ex.HandleException();
            }
        }
    }
}
