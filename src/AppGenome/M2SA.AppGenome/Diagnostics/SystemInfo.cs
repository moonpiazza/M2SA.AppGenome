using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Text;
using System.Net;
using System.Threading;

namespace M2SA.AppGenome.Diagnostics
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class SystemInfo 
    {
        #region Static Members

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static string GetServerIP()
        {
            var serverIP = string.Empty;

            var machineName = System.Environment.MachineName;
            var ipList = Dns.GetHostEntry(machineName).AddressList;
            var ips = new string[ipList.Length];
            for (var i = 0; i < ipList.Length; i++)
            {
                ips[i] = ipList[i].ToString();
            }
            serverIP = string.Join(",", ips);

            return serverIP;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static SystemInfo GetInfo()
        {
             var sysInfo = new SystemInfo();
            sysInfo.ServerIP = GetServerIP();
            AppendSysInfo(sysInfo, ".", Process.GetCurrentProcess().ProcessName);

            var currentThread = Thread.CurrentThread;
            sysInfo.ThreadId = currentThread.ManagedThreadId;
            sysInfo.ThreadName = currentThread.Name;
            sysInfo.ThreadPool = AppInstance.GetThreadPool().ActiveThreads;

            return sysInfo;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sourceIP"></param>
        /// <param name="processName"></param>
        /// <returns></returns>
        public static SystemInfo GetInfo(string sourceIP, string processName)
        {
            var targetIP = sourceIP;
            var sysInfo = new SystemInfo();
            sysInfo.ServerIP = sourceIP;
            if (string.IsNullOrEmpty(targetIP) == false && targetIP.IndexOf(",") != -1)
            {
                targetIP = targetIP.Split(',')[0];
            }

            AppendSysInfo(sysInfo, targetIP, processName);

            return sysInfo;
        }

        static SystemInfo AppendSysInfo(SystemInfo sysInfo, string targetIP, string processName)
        {
            try
            {
                sysInfo.TotalMemory = PerfmonCounterManager.GetCounterItemValue("Committed Bytes", string.Empty, "Memory", targetIP);
                sysInfo.TotalCPU = PerfmonCounterManager.GetSampleItemValue("% Processor Time", "_Total", "Processor", targetIP);                
                if (string.IsNullOrEmpty(processName) == false)
                {
                    sysInfo.ProcessName = processName;
                    sysInfo.ProcessMemory = PerfmonCounterManager.GetCounterItemValue("Private Bytes", sysInfo.ProcessName, "Process", targetIP);
                    sysInfo.ProcessCPU = PerfmonCounterManager.GetSampleItemValue("% Processor Time", sysInfo.ProcessName, "Process", targetIP);                    
                    sysInfo.ProcessThreadCount = PerfmonCounterManager.GetCounterItemValue("Thread Count", sysInfo.ProcessName, "Process", targetIP);
                }

            }
            catch (Exception ex)
            {
                sysInfo.TotalCPU = -1.0f;
                sysInfo.ProcessCPU = -1.0f;
            }
            
            return sysInfo;
        }

        #endregion

        #region ISysInfo Members

        /// <summary>
        /// 
        /// </summary>
        public string ServerIP
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public float TotalCPU
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public long TotalMemory
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public string ProcessName
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public float ProcessCPU
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public long ProcessMemory
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public long ProcessThreadCount
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public string ThreadName
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public int ThreadId
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public int ThreadPool
        {
            get;
            set;
        }

        #endregion
    }

}
