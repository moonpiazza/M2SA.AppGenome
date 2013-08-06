using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using M2SA.AppGenome.Configuration;
using M2SA.AppGenome.Logging.Formatters;

namespace M2SA.AppGenome.Logging.Listeners
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public abstract class ListenerBase : ResolveObjectBase, IListener
    {
        #region IListener 成员

        /// <summary>
        /// 
        /// </summary>
        public string Name
        {
            get;
            protected set;
        }

        /// <summary>
        /// 
        /// </summary>
        public string Type
        {
            get;
            protected set;
        }

        /// <summary>
        /// 
        /// </summary>
        public bool SupportAsync
        {
            get;
            protected set;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entry"></param>
        public abstract void WriteMessage(ILogEntry entry);
        

        #endregion

        /// <summary>
        /// 
        /// </summary>
        public ListenerBase()
        {
            this.SupportAsync = true;
        }
    }
}
