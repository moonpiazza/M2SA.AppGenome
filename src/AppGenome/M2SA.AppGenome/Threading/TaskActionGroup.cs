using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace M2SA.AppGenome.Threading
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TaskActionGroup<T> : ITaskActionGroup<T>
    {
        static readonly object syncRoot = new object();
        readonly IWorkItemsGroup threadPool = null;

        #region Properties

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
        public int Concurrency
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
        public Action<T> Action
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public Queue<T> Items
        {
            get;
            private set;
        }

        /// <summary>
        /// 
        /// </summary>
        public TimeSpan PocessInterval
        {
            get;
            set;
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="pocessInterval"></param>
        /// <param name="action"></param>
        /// <param name="canCancel"></param>
        public TaskActionGroup(string name, TimeSpan pocessInterval, Action<T> action, bool canCancel)
            : this(name, pocessInterval, action, canCancel,1)
        {            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="pocessInterval"></param>
        /// <param name="action"></param>
        /// <param name="canCancel"></param>
        /// <param name="concurrency"></param>
        public TaskActionGroup(string name, TimeSpan pocessInterval, Action<T> action, bool canCancel, int concurrency)
        {
            this.CanCancel = canCancel;
            this.Name = name;
            this.PocessInterval = pocessInterval;
            this.Action = action;

            this.Items = new Queue<T>(32);

            this.Concurrency = concurrency;
            this.threadPool = AppInstance.GetThreadPool().CreateWorkItemsGroup(this.Concurrency);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        public void Enqueue(T item)
        {
            lock (syncRoot)
            {
                this.Items.Enqueue((T)item);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Invoke()
        {
            T[] items = null;

            lock (syncRoot)
            {
                if (this.Items.Count > 0)
                {
                    items = this.Items.ToArray();
                    this.Items.Clear();
                }
            }

            if (items != null && items.Length > 0)
            {
                for (var i = 0; i < items.Length; i++)
                {
                    this.Invoke(items[i]);
                }
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="waitInterval"></param>
        /// <returns></returns>
        public bool Trigger(TimeSpan waitInterval)
        {
            return waitInterval >= this.PocessInterval;
        }

        void Invoke(T item)
        {
            this.threadPool.QueueWorkItem<T>(this.Action, item);
        }
    }
}
