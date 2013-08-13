using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace M2SA.AppGenome.Queues.Tests.TestObjects
{
    [Serializable]
    public class TestMessage
    {
        public string Name
        {
            get;
            set;
        }

        public override bool Equals(object obj)
        {
            if (obj is TestMessage)
            {
                return this.Name == ((TestMessage)obj).Name;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return this.Name.GetHashCode();
        }
    }
}
