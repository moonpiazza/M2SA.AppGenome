using System;
using System.Collections.Generic;
using System.Threading;
using System.Text;
using System.Linq;
using M2SA.AppGenome;
using M2SA.AppGenome.Threading;

namespace M2SA.AppGenome.Threading
{
    /// <summary>
    /// 
    /// </summary>
    public class TimeAction : ITaskAction
    {
        CronExpression cronExpression = null;
        TimeSpan waitSpan = TimeSpan.Zero;
        DateTime nextFireTime = DateTime.MaxValue;

        readonly IWorkItemsGroup threadPool = null;

        /// <summary>
        /// 是否可以被取消
        /// </summary>
        public bool CanCancel
        {
            get;
            private set;
        }

        /// <summary>
        /// 
        /// </summary>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public Action<DateTime> Action
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public int Concurrency
        {
            get;
            private set;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="cronExpressionValue"></param>
        /// <param name="action"></param>
        /// <param name="concurrency"></param>
        public TimeAction(string name, string cronExpressionValue, Action<DateTime> action, int concurrency)
        {
            this.Name = name;
            this.cronExpression = new CronExpression(cronExpressionValue);
            this.Action = action;
            this.ResetNextFireTime();

            this.Concurrency = concurrency;
            this.threadPool = AppInstance.GetThreadPool().CreateWorkItemsGroup(this.Concurrency);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="cronExpressionValue"></param>
        /// <param name="action"></param>
        public TimeAction(string name, string cronExpressionValue, Action<DateTime> action)
            : this(name, cronExpressionValue, action, 1)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="cronExpressionValue"></param>
        /// <param name="action"></param>
        /// <param name="canCancel"></param>
        public TimeAction(string name, string cronExpressionValue, Action<DateTime> action, bool canCancel)
            : this(name, cronExpressionValue, action)
        {
            this.CanCancel = canCancel;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="pocessInterval"></param>
        /// <param name="action"></param>
        /// <param name="concurrency"></param>
        public TimeAction(string name, TimeSpan pocessInterval, Action<DateTime> action, int concurrency)
        {
            this.Name = name;
            this.waitSpan = pocessInterval;
            this.Action = action;
            this.ResetNextFireTime();

            this.Concurrency = concurrency;
            this.threadPool = AppInstance.GetThreadPool().CreateWorkItemsGroup(this.Concurrency);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="pocessInterval"></param>
        /// <param name="action"></param>
        public TimeAction(string name, TimeSpan pocessInterval, Action<DateTime> action)
            : this(name, pocessInterval, action, 1)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="pocessInterval"></param>
        /// <param name="action"></param>
        /// <param name="canCancel"></param>
        public TimeAction(string name, TimeSpan pocessInterval, Action<DateTime> action, bool canCancel) 
            : this(name, pocessInterval, action)
        {
            this.CanCancel = canCancel;
        }

        #region ITaskAction 成员

        /// <summary>
        /// 
        /// </summary>
        public void Invoke()
        {
            if (AppInstance.GetThreadPool().IsShuttingdown)
                return;

            this.threadPool.QueueWorkItem<DateTime>(this.Action, DateTime.Now);
            this.ResetNextFireTime();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="waitInterval"></param>
        /// <returns></returns>
        public bool Trigger(TimeSpan waitInterval)
        {
            return this.nextFireTime <= DateTime.Now;
        }

        #endregion      
  
        void ResetNextFireTime()
        {
            if (this.cronExpression == null)
            {
                if (this.nextFireTime == DateTime.MaxValue)
                    this.nextFireTime = DateTime.Now + this.waitSpan;
                else
                    this.nextFireTime = this.nextFireTime + this.waitSpan;
            }
            else
            {
                if (this.nextFireTime == DateTime.MaxValue)
                    this.nextFireTime = this.cronExpression.GetNextValidTimeAfter(DateTime.Now).Value;
                else
                    this.nextFireTime = this.cronExpression.GetNextValidTimeAfter(this.nextFireTime).Value;
            }
        }
    }
}
