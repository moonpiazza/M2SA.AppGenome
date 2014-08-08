using System;
using System.Collections.Generic;
using  M2SA.AppGenome.Configuration;

namespace M2SA.AppGenome.AppHub
{
    /// <summary>
    /// 
    /// </summary>
    public class ApplicationHub : ResolveObjectBase, IExtensionApplication
    {
        static readonly object SyncRoot = new object();

        /// <summary>
        /// 
        /// </summary>
        public bool AsyncStart { get { return false; } }

        /// <summary>
        /// 
        /// </summary>
        public IList<IExtensionApplication> Extensions { get; private set; }

        private ApplicationHub()
        {
            this.Extensions = new List<IExtensionApplication>(4);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="onwer"></param>
        /// <param name="args"></param>
        public void OnInit(ApplicationHost onwer, AppHub.CommandArguments args)
        {
            foreach (var item in this.Extensions)
            {
                item.OnInit(onwer, args);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="onwer"></param>
        /// <param name="args"></param>
        public void OnStart(ApplicationHost onwer, AppHub.CommandArguments args)
        {
            foreach (var extension in this.Extensions)
            {
                if (false == extension.AsyncStart)
                    extension.OnStart(onwer, args);
            }
            foreach (var extension in this.Extensions)
            {
                if (true == extension.AsyncStart)
                    extension.OnStart(onwer, args);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="onwer"></param>
        /// <param name="args"></param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        public void OnStop(ApplicationHost onwer, AppHub.CommandArguments args)
        {
            foreach (var item in this.Extensions)
            {
                try
                {
                    item.OnStop(onwer, args);
                }
                catch (Exception ex)
                {
                    new FatalException(ex).HandleException();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        public void Register<T>(T item) where T : IExtensionApplication
        {
            lock (SyncRoot)
            {
                this.Extensions.Add(item);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        public void UnRegister<T>(T item) where T : IExtensionApplication
        {
            lock (SyncRoot)
            {
                this.Extensions.Remove(item);
            }
        }
    }
}
