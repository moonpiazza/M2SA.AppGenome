using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace M2SA.AppGenome
{
    /// <summary>
    /// 
    /// </summary>
    public enum PersistentState
    {
        /// <summary>
        /// 瞬态:在数据库中没有与之相对应的记录
        /// </summary>
        Transient,

        /// <summary>
        /// 持久态:在数据库中存在与之相对应的记录
        /// </summary>
        Persistent,

        /// <summary>
        /// 
        /// </summary>
        Detached,
        
        /// <summary>
        /// 已删除:在数据库中已删除相对应的记录
        /// </summary>
        Deleted
    }
}
