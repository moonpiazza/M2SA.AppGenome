using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.ServiceProcess;
using System.Text;
using M2SA.AppGenome.Reflection;
using M2SA.AppGenome.AppHub;

namespace M2SA.AppGenome.ServiceHost
{
    partial class WinService : ServiceBase
    {
        ApplicationHost applicationHost = null;

        public WinService()
        {
            InitializeComponent();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        protected override void OnStart(string[] args)
        {
            try
            {
                applicationHost = ApplicationHost.GetInstance(args);
                ObjectIOCFactory.GetSingleton<ApplicationHub>().Register(ObjectIOCFactory.GetSingleton<ApplicationContainer>());
                applicationHost.Start();
            }
            catch(Exception ex)
            {
                var log = string.Format("{0}_OnStartException.log", DateTime.Now.ToString("yyyyMMddHH"));
                FileHelper.WriteInfo(log, ex.ToText());
            }
        }

        protected override void OnStop()
        {
            applicationHost.Stop();
        }
    }
}
