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
    public interface ITaskActionGroup<T> : ITaskAction
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        void Enqueue(T item);
    }
}
