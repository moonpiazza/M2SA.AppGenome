using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace M2SA.AppGenome.Logging.Tests.TestObjects
{
    public class MemoryLogSource
    {
        static readonly object syncObject = new object();

        IDictionary<string, IList<object>> LogSource = null;

        public MemoryLogSource()
        {
            this.LogSource = new Dictionary<string, IList<object>>(10);
        }

        public void Store(string groupName, object entry)
        {
            if (this.LogSource.ContainsKey(groupName) == false)
            {
                lock (syncObject)
                {
                    if (this.LogSource.ContainsKey(groupName) == false)
                    {
                        this.LogSource[groupName] = new List<object>(100);
                    }
                }
            }

            lock (syncObject)
            {
                this.LogSource[groupName].Add(entry);
            }
        }

        public IList<object> GetLogInfos(string guoupName)
        {
            if (this.LogSource.ContainsKey(guoupName))
            {
                return this.LogSource[guoupName];
            }
            return new List<object>(0);
        }

        public void Clear(string guoupName)
        {
            if (this.LogSource.ContainsKey(guoupName))
            {
                lock (syncObject)
                {
                    this.LogSource[guoupName].Clear();
                }
            }
        }

    }
}
