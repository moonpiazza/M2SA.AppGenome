using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace M2SA.AppGenome.Configuration
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class ResolveObjectBase : IResolveObject
    {
        #region IResolveObject 成员

        /// <summary>
        /// 
        /// </summary>
        /// <param name="config"></param>
        public virtual void Initialize(IConfigNode config)
        {
            this.DeserializeObject(config);
        }

        #endregion
    }
}
