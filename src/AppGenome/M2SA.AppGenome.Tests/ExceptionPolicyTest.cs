using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Xml;
using System.Diagnostics;
using System.Threading;
using NUnit.Framework;
using M2SA.AppGenome;

namespace M2SA.AppGenome.Tests
{
    [TestFixture]
    public class ExceptionPolicyTest : M2SA.AppGenome.Tests.TestBase
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes"), Test]
        public void TestException()
        {            
            try
            {
                int.Parse("a");
            }
            catch(Exception ex)
            {
                ex.HandleException();
            }
        }

        [Test]
        public void TestExceptionAction()
        {
            Action action = () => {
                int.Parse("a");
            };

            action.Handle();
        }
    }
}
