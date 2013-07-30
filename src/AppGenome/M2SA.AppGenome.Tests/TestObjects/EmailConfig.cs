using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using M2SA.AppGenome.Configuration;

namespace M2SA.AppGenome.Tests.TestObjects
{
    public class EmailConfig : ResolveObjectBase
    {
        public string PopServer { get; set; }

        public int PopServerPort { get; set; }

        public string SmtpServer { get; set; }

        public int SmtpServerPort { get; set; }


        public string UserName { get; set; }

        public string Password { get; set; }

        public bool EnableSSL { get; set; }

        public override void Initialize(IConfigNode config)
        {
            this.PopServerPort = 110;
            this.SmtpServerPort = 25;

            base.Initialize(config);
        }
    }
}
