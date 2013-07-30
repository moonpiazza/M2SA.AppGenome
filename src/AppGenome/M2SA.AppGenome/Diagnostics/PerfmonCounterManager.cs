using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Diagnostics;

namespace M2SA.AppGenome.Diagnostics
{
    /// <summary>
    /// 
    /// </summary>
    public static class PerfmonCounterManager
    {
        static object syscObject = new object();
        static bool isStartListen = false;
        static IDictionary<string, PerfmonCounter> PerfmonCounterMap = new Dictionary<string,PerfmonCounter>() ;
        static IDictionary<string, KeyValuePair<float, SampleCounterProxy>> SampleCounterMap = new Dictionary<string, KeyValuePair<float, SampleCounterProxy>>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="categoryName"></param>
        /// <param name="machineName"></param>
        /// <returns></returns>
        public static PerfmonCounterCategory GetCounterCategory(string categoryName, string machineName)
        {
            if (PerformanceCounterCategory.Exists(categoryName, machineName) == false)
                return null;

            var counterCategory = new PerfmonCounterCategory(categoryName, machineName);
            counterCategory.CategoryName = categoryName;

            var sysCategory = new PerformanceCounterCategory(categoryName, machineName);
            var sysInstances = sysCategory.GetInstanceNames();

            foreach (var instanceName in sysInstances)
            {
                var perfmonCounter = new PerfmonCounter(instanceName, categoryName);
                var counterItems = sysCategory.GetCounters(instanceName);

                foreach (var counterItem in counterItems)
                {
                    perfmonCounter.CounterData.Add(counterItem.CounterName, counterItem);
                }

                counterCategory.Instances.Add(instanceName, perfmonCounter);
            }

            if (sysInstances.Length == 0)
            {
                var categoryCounters = sysCategory.GetCounters();
                if (categoryCounters.Length > 0)
                {
                    var instanceName = string.Empty;
                    var perfmonCounter = new PerfmonCounter(instanceName, categoryName);
                    var counterItems = sysCategory.GetCounters(instanceName);

                    foreach (var counterItem in counterItems)
                    {
                        perfmonCounter.CounterData.Add(counterItem.CounterName, counterItem);
                    }

                    counterCategory.Instances.Add(instanceName, perfmonCounter);
                }
            }

            return counterCategory;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="instanceName"></param>
        /// <param name="categoryName"></param>
        /// <param name="machineName"></param>
        /// <returns></returns>
        public static PerfmonCounter GetCounterInstance(string instanceName, string categoryName, string machineName)
        {
            PerfmonCounter counterInstance = null;
            string counterKey = string.Format("{0}[{1}.{2}]", instanceName, machineName, categoryName);
            if (PerfmonCounterMap.ContainsKey(counterKey))
            {
                counterInstance = PerfmonCounterMap[counterKey];
            }
            else
            {
                lock (syscObject)
                {
                    if (PerfmonCounterMap.ContainsKey(counterKey))
                    {
                        counterInstance = PerfmonCounterMap[counterKey];
                    }
                    else if (string.IsNullOrEmpty(instanceName) || PerformanceCounterCategory.InstanceExists(instanceName, categoryName, machineName))
                    {
                        var counterCategory = GetCounterCategory(categoryName, machineName);
                        counterInstance = counterCategory.Instances[instanceName];
                        PerfmonCounterMap[counterKey] = counterInstance;
                    }
                }
            }
            return counterInstance;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="itemName"></param>
        /// <param name="instanceName"></param>
        /// <param name="categoryName"></param>
        /// <param name="machineName"></param>
        /// <returns></returns>
        public static long GetCounterItemValue(string itemName, string instanceName, string categoryName, string machineName)
        {
            var counterInstance = GetCounterInstance(instanceName, categoryName, machineName);
            if (counterInstance == null)
                return 0;

            var counterItem = (PerformanceCounter)counterInstance.CounterData[itemName];
            var itemValue = counterItem.RawValue;

            return itemValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="itemName"></param>
        /// <param name="instanceName"></param>
        /// <param name="categoryName"></param>
        /// <param name="machineName"></param>
        /// <returns></returns>
        public static float GetCounterItemNextValue(string itemName, string instanceName, string categoryName, string machineName)
        {
            var counterInstance = GetCounterInstance(instanceName, categoryName, machineName);
            if (counterInstance == null)
                return 0;

            var counterItem = (PerformanceCounter)counterInstance.CounterData[itemName];
            var itemValue = counterItem.NextValue();
            System.Threading.Thread.Sleep(1000);
            itemValue = counterItem.NextValue();


            return itemValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="itemName"></param>
        /// <param name="instanceName"></param>
        /// <param name="categoryName"></param>
        /// <param name="machineName"></param>
        /// <returns></returns>
        public static float GetSampleItemValue(string itemName, string instanceName, string categoryName, string machineName)
        {
            string counterKey = string.Format("{0}.{1}.{2}.{3}", machineName, categoryName, instanceName, itemName);
            if (SampleCounterMap.ContainsKey(counterKey))
            {
                return SampleCounterMap[counterKey].Key;
            }
            else
            {
                RegisterSampleCounterListener(itemName, instanceName, categoryName, machineName);
                Thread.Sleep(120);
                return SampleCounterMap[counterKey].Key;
            }
        }

        #region SampleCounterProxy

        static void StartListen()
        {
            if (isStartListen == false)
            {
                lock (syscObject)
                {
                    if (isStartListen == false)
                    {
                        isStartListen = true;
                        var t = new Thread(RefreshSampleCounterMap);
                        t.Start();
                    }
                }
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        static void RefreshSampleCounterMap()
        {
            string[] keys = null;

            if (SampleCounterMap.Keys.Count > 0)
            {                
                lock (syscObject)
                {
                    keys = new string[SampleCounterMap.Keys.Count];
                    SampleCounterMap.Keys.CopyTo(keys, 0);
                }

                foreach (var key in keys)
                {
                    try
                    {
                        var counter = SampleCounterMap[key].Value;
                        var counterInstance = PerfmonCounterManager.GetCounterInstance(counter.InstanceName, counter.CategoryName, counter.MachineName);
                        if (counterInstance == null)
                            continue;

                        var counterItem = (PerformanceCounter)counterInstance.CounterData[counter.ItemName];
                        var itemValue = counterItem.NextValue();
                    }
                    catch (Exception ex)
                    {
                        new FatalException(ex).HandleException();
                    }
                }
            }

            Thread.Sleep(1000);

            if (keys != null && keys.Length > 0)
            {
                foreach (var key in keys)
                {
                    try
                    {
                        var counter = SampleCounterMap[key].Value;
                        var counterInstance = PerfmonCounterManager.GetCounterInstance(counter.InstanceName, counter.CategoryName, ".");
                        if (counterInstance == null)
                            continue;

                        var counterItem = (PerformanceCounter)counterInstance.CounterData[counter.ItemName];
                        var itemValue = counterItem.NextValue();
                        SampleCounterMap[key] = new KeyValuePair<float, SampleCounterProxy>(itemValue, counter);
                    }
                    catch (Exception ex)
                    {
                        new FatalException(ex).HandleException();
                    }
                }
            }

            RefreshSampleCounterMap();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="itemName"></param>
        /// <param name="instanceName"></param>
        /// <param name="categoryName"></param>
        /// <param name="machineName"></param>
        static void RegisterSampleCounterListener(string itemName, string instanceName, string categoryName, string machineName)
        {
            StartListen();

            string counterKey = string.Format("{0}.{1}.{2}.{3}", machineName, categoryName, instanceName, itemName);
            if (SampleCounterMap.ContainsKey(counterKey) == false)
            {
                var proxy = new SampleCounterProxy() {
                    MachineName = machineName,
                    CategoryName = categoryName,
                    InstanceName = instanceName,
                    ItemName = itemName
                };

                lock (syscObject)
                {
                    if (SampleCounterMap.ContainsKey(counterKey) == false)
                    {
                        SampleCounterMap.Add(counterKey, new KeyValuePair<float, SampleCounterProxy>(0, proxy));
                    }
                }                
            }            
        }

        /// <summary>
        /// 
        /// </summary>
        public class SampleCounterProxy
        {
            /// <summary>
            /// 
            /// </summary>
            public string MachineName
            {
                get;
                set;
            }

            /// <summary>
            /// 
            /// </summary>
            public string CategoryName
            {
                get;
                set;
            }

            /// <summary>
            /// 
            /// </summary>
            public string InstanceName
            {
                get;
                set;
            }

            /// <summary>
            /// 
            /// </summary>
            public string ItemName
            {
                get;
                set;
            }
        }

        #endregion
    }
}
