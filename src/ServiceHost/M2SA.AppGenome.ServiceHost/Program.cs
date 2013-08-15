using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Reflection;
using System.ServiceProcess;
using System.Threading;
using M2SA.AppGenome.AppHub;

namespace M2SA.AppGenome.ServiceHost
{
    class Program
    {
        static ApplicationHost applicationHost = null;

        static void Main(string[] args)
        {
            var hostConfig = ObjectIOCFactory.GetSingleton<ServiceHostConfig>();
            var commandArguments = new CommandArguments(args);
            if (commandArguments.ContainsArgument("service"))
                hostConfig.Host.RunInService = true;
            if (commandArguments.ContainsArgument("debug"))
                hostConfig.App.Debug = true;

            if (hostConfig.Host.RunInService)
            {
                ServiceBase.Run(new WinService());
            }
            else
            {
                var exitHanlder = new ConsoleExitHanlder();
                exitHanlder.Exit += new EventHandler((source, e) => applicationHost.Stop());

                applicationHost = ApplicationHost.GetInstance(args);
                ObjectIOCFactory.GetSingleton<ApplicationHub>().Register(ObjectIOCFactory.GetSingleton<ApplicationContainer>());
                applicationHost.Start();
                ExecuteSystemCommand();
            }
        }

        static void ExecuteSystemCommand()
        {
            var exitFlag = false;
            while (exitFlag == false)
            {
                var command = Console.ReadLine();
                if(string.IsNullOrEmpty(command))
                    return;

                var commandType = CommandType.None;

                if (Enum.TryParse(command, true, out commandType))
                {
                    switch (commandType)
                    {
                        case CommandType.Restart:
                            {
                                ObjectIOCFactory.GetSingleton<ApplicationContainer>().Restart();
                                break;
                            }
                        case CommandType.Exit:
                            {
                                exitFlag = true;
                                applicationHost.Stop();
                                break;
                            }
                        default:
                            {
                                return;
                            }
                    }
                }

            }  
        }
    }
}
