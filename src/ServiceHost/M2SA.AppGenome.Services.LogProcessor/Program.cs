using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using M2SA.AppGenome.AppHub;

namespace M2SA.AppGenome.Services.LogProcessor
{
    class Program
    {
        static void Main(string[] args)
        {
            AppInstance.RegisterTypeAlias<QueueListener>(typeof(QueueListener).Name);
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
