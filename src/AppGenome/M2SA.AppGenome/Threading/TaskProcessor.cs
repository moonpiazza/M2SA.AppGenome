using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;
using System.Threading;
using System.Diagnostics;

namespace M2SA.AppGenome.Threading
{
    /// <summary>
    /// 后台任务处理器
    /// </summary>
    public class TaskProcessor : IExtensionApplication
    {
        readonly static object syncObject = new object();

        TimeSpan onceInterval = new TimeSpan(0, 0, 1);
        IDictionary<string, ITaskAction> actionMap = null;
        IDictionary<string, TimeSpan> waitIntervalMap = null;

        /// <summary>
        /// 
        /// </summary>
        public TaskProcessor()
        {
            this.actionMap = new Dictionary<string, ITaskAction>();
            this.waitIntervalMap = new Dictionary<string, TimeSpan>(); 
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <param name="action"></param>
        public void RegisterAction<T>(string name, Action<T> action)
        {
            this.RegisterAction<T>(name, action, onceInterval);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>        
        /// <param name="action"></param>
        /// <param name="pocessInterval"></param>
        public void RegisterAction<T>(string name, Action<T> action, TimeSpan pocessInterval)
        {
            this.RegisterAction<T>(name, action, pocessInterval, false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <param name="action"></param>
        /// <param name="pocessInterval"></param>
        /// <param name="canCancel"></param>
        public void RegisterAction<T>(string name, Action<T> action, TimeSpan pocessInterval, bool canCancel)
        {
            var taskAction = new TaskActionGroup<T>(name, pocessInterval, action, canCancel);
            this.RegisterAction(name, taskAction);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="taskAction"></param>
        public void RegisterAction(string name, ITaskAction taskAction)
        {
            lock (syncObject)
            {
                if (this.actionMap.ContainsKey(name) == false)
                {
                    this.waitIntervalMap[name] = TimeSpan.Zero;
                    this.actionMap[name] = taskAction;                    
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <param name="item"></param>
        public void Process<T>(string name, T item)
        {
            if (this.actionMap.ContainsKey(name) == false)
            {
                throw new ArgumentOutOfRangeException("name", name, "not find the ITaskActionGroup");
            }
            var taskAction = this.actionMap[name];
            if (taskAction is ITaskActionGroup<T>)
            {
                ((ITaskActionGroup<T>)taskAction).Enqueue(item);
            }
            else
            {
                throw new ArgumentOutOfRangeException("name", name, "not find the TaskActionGroup");
            }
        }

        void Process()
        {
            while (true)
            {
                lock (syncObject)
                {
                    var stopwatch = Stopwatch.StartNew();
                    var actionNames = this.actionMap.Keys;
                    foreach (var aName in actionNames)
                    {
                        var taskAction = this.actionMap[aName];
                        if (taskAction.Trigger(this.waitIntervalMap[aName]))
                        {
                            this.Invoke(aName, taskAction);
                        }
                    }

                    stopwatch.Stop();
                    var waitInterval = stopwatch.Elapsed;
                    if (waitInterval < this.onceInterval)
                    {
                        Thread.Sleep((int)(this.onceInterval - waitInterval).TotalMilliseconds);
                        waitInterval = onceInterval;
                    }

                    foreach (var aName in actionNames)
                    {
                        this.waitIntervalMap[aName] += waitInterval;
                    }
                }
            }
        }

        void Invoke(string aName, ITaskAction taskAction)
        {
            this.waitIntervalMap[aName] = TimeSpan.Zero;
            taskAction.Invoke();
        }

        #region IExtensionApplication 成员

        void IExtensionApplication.OnInit(ExtensibleApplication onwer, M2SA.AppGenome.AppHub.CommandArguments args)
        {
            
        }

        void IExtensionApplication.OnStart(ExtensibleApplication onwer, M2SA.AppGenome.AppHub.CommandArguments args)
        {
            LogManager.GetLogger().Debug("---------- TaskProcessor.Start... ----------");   

            ThreadStart ts = new ThreadStart(this.Process);
            Thread t = new Thread(ts);
            t.Start();
        }

        void IExtensionApplication.OnStop(ExtensibleApplication onwer, M2SA.AppGenome.AppHub.CommandArguments args)
        {
            var stop = Stopwatch.StartNew();

            LogManager.GetLogger().Debug("---------- TaskProcessor.BeginStop... ----------");  
            lock (syncObject)
            {
                var threadPool = AppInstance.GetThreadPool();

                #region Resize ThreadPool

                var minFactor = this.actionMap.Count + 3;
                if (minFactor > threadPool.MaxThreads)
                {
                    var shutdownMaxThreads = 20;
                    if (threadPool.MaxThreads < shutdownMaxThreads)
                    {
                        threadPool.MaxThreads = shutdownMaxThreads;
                        threadPool.MinThreads = minFactor > shutdownMaxThreads ? shutdownMaxThreads : minFactor;
                    }
                }  

                #endregion

                foreach (var item in this.actionMap)
                {
                    var taskAction = item.Value;
                    if (taskAction.CanCancel == false)
                    {
                        this.Invoke(item.Key, taskAction);
                    }
                }

                threadPool.WaitForIdle();
            }
            stop.Stop();
            LogManager.GetLogger().Debug("---------- TaskProcessor.Stoped [{0}]... ----------", stop.Elapsed);   
        }

        #endregion
    }
}
