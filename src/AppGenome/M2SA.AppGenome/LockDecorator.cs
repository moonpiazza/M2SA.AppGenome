using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace M2SA.AppGenome
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class LockDecorator<T>
    {
        private static readonly object locker = new object();


        /// <summary>
        /// 
        /// </summary>
        /// <param name="actions"></param>
        public void Work(params Action[] actions)
        {
            if (null == actions)
                throw new ArgumentNullException("actions");

            lock (locker)
            {
                foreach (var action in actions)
                {
                    action();
                }
            }
        }
    }
}
