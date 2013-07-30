using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Diagnostics;
using System.Threading;
using NUnit.Framework;
using M2SA.AppGenome.Configuration;

namespace M2SA.AppGenome.Tests.TestObjects
{
    public interface IDomainSetting : IResolveObject
    {
        DomainRule DefaultRule { get; set; }
        IDictionary<string, DomainRule> DomainRules { get; set; }
    }

    public class DomainSetting : ResolveObjectBase, IDomainSetting
    {
        public DomainRule DefaultRule { get; set; }

        public IDictionary<string, DomainRule> DomainRules { get; set; }
    }

    public class DomainRule
    {
        public string Name { get; set; }

        public string Encode { get; set; }

        public IDictionary<string, string> EnterRules { get; set; }

        public bool IsSummaryFeed { get; set; }
    }
}
