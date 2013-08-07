using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace M2SA.AppGenome.Cache
{
    /// <summary>
    /// ICacheNotify的空实现
    /// </summary>
    public class EmptyCacheNotify : ICacheNotify
    {
        #region ICacheNotify 成员

        /// <summary>
        /// 
        /// </summary>
        public bool NeedItemHandler
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cache"></param>
        public void SetCache(ICache cache) { /* empty action */ }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        public void OnBeforeGet(string key) { /* empty action */ }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void OnAfterGet(string key, ref object value) { /* empty action */ }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void OnBeforeSet(string key, object value) { /* empty action */ }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void OnAfterSet(string key, object value) { /* empty action */ }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        public void OnBeforeRemove(string key) { /* empty action */ }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        public void OnAfterRemove(string key) { /* empty action */ }

        #endregion
    }
}
