using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace M2SA.AppGenome.Tests.TestObjects
{
    public class TestApplication : IExtensionApplication
    {
        public bool AsyncStart { get; set; }

        public string Name { get; set; }

        public string TestId { get; set; }

        public TestApplication()
        {
            this.TestId = TestHelper.RandomizeString();
        }

        public void OnInit(ApplicationHost onwer, AppHub.CommandArguments args)
        {
            Console.WriteLine("TestApplication[{0}].Init - {1}", this.Name,this.TestId);
        }

        public void OnStart(ApplicationHost onwer, AppHub.CommandArguments args)
        {
            Console.WriteLine("TestApplication[{0}].Start - {1}", this.Name, this.TestId);
        }

        public void OnStop(ApplicationHost onwer, AppHub.CommandArguments args)
        {
            Console.WriteLine("TestApplication[{0}].Stop - {1}", this.Name, this.TestId);
        }
    }
}
