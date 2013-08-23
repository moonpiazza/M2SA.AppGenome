using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using M2SA.AppGenome.AppHub;

namespace M2SA.AppGenome.Services.MemCachedStats
{
    class Program
    {
        static void Main(string[] args)
        {
            AppInstance.RegisterTypeAlias<LoadMemCachedStatsListener>();
            ObjectIOCFactory.GetSingleton<ApplicationHub>().Register<ExitCommandListener>(new ExitCommandListener());
            ApplicationHost.GetInstance(args).Start();

            var exitHanlder = new ConsoleExitHanlder();
            exitHanlder.Exit += new EventHandler((source, e) =>
            {
                ApplicationHost.GetInstance().Stop();
            });

            Console.ReadLine();
        }
    }
}
