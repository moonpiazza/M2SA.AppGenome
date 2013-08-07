using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace M2SA.AppGenome.Cache.Tests.TestObjects
{
    public class LoadMemoryItemHander : ILoadDataHandler
    {
        private static readonly object sync = new object();

        private IDictionary<string, string> source = new Dictionary<string, string>();

        public object LoadData(string key)
        {
            if (false == source.ContainsKey(key))
            {
                lock (sync)
                {
                    if (false == source.ContainsKey(key))
                        source[key] = string.Concat(key, "--", TestHelper.RandomizeString());
                }
            }

            string val = source[key];
            return val;
        }
    }
}
