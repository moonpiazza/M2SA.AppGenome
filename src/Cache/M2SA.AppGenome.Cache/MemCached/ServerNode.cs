using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace M2SA.AppGenome.Cache.MemCached
{
    /// <summary>
    /// 
    /// </summary>
    public class ServerNode
    {
        /// <summary>
        /// 
        /// </summary>
        public string Address
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public ServerScope Scope
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public IList<ServerNode> Slaves
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public ServerNode()
        {
            this.Scope = ServerScope.Read | ServerScope.Write;
        }
    }
}
