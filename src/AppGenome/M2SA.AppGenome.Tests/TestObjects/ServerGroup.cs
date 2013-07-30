using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using NUnit.Framework;
using M2SA.AppGenome.Configuration;
using M2SA.AppGenome.Reflection;

namespace M2SA.AppGenome.Tests.TestObjects
{
    public class ServerGroup : ResolveObjectBase
    {
        #region Server

        public class Server
        {
            public string ServerName { get; set; }
            public string ServerIP { get; set; }
            public int ServicePort { get; set; }
        }

        #endregion


        public string GroupName { get; set; }
        public IList<Server> Servers { get; set; }

        public override void Initialize(IConfigNode config)
        {
            this.Servers = new List<Server>(4);
            base.Initialize(config);
        }
    }
}
