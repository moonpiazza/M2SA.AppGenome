using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace M2SA.AppGenome.Data
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class EntityBase<TId> : IEntity<TId>
    {
        private TId id = default(TId);

        /// <summary>
        /// 
        /// </summary>
        [NonSerializedProperty]
        public TId Id
        {
            get { return this.id; }
            set { this.id = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public PersistentState PersistentState { get; set; }
    }
}
