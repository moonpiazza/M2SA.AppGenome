using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Reflection;

namespace M2SA.AppGenome.AppHub
{
    /// <summary>
    /// 
    /// </summary>
    public class ConsoleExitHanlder
    {
        internal static class NativeMethods
        {
            [DllImport("kernel32.dll")]
            internal static extern bool SetConsoleCtrlHandler(ConsoleCtrlDelegate HandlerRoutine, bool Add);
        }

        const int CTRL_C_EVENT = 0;
        const int CTRL_BREAK_EVENT = 1;
        const int CTRL_CLOSE_EVENT = 2;
        const int CTRL_LOGOFF_EVENT = 5;
        const int CTRL_SHUTDOWN_EVENT = 6;

        internal delegate bool ConsoleCtrlDelegate(int dwCtrlType);

        /// <summary>
        /// 
        /// </summary>
        public event EventHandler Exit;

        private ConsoleCtrlDelegate consoleCtrlDelegate;

        /// <summary>
        /// 
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        public ConsoleExitHanlder()
        {
            try
            {
                this.consoleCtrlDelegate = new ConsoleCtrlDelegate(HandlerRoutine);
                NativeMethods.SetConsoleCtrlHandler(this.consoleCtrlDelegate, true);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Set SetConsoleCtrlHandler Error : {0}", ex.Message);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        bool HandlerRoutine(int CtrlType)
        {            
            switch (CtrlType)
            {
                case CTRL_CLOSE_EVENT:
                    if (this.Exit != null)
                    {
                        try
                        {
                            this.Exit(this, new EventArgs());
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Set SetConsoleCtrlHandler Error : {0}", ex.Message);
                        }
                    }
                    break;
            }
            return false;
        }      
    }
}
