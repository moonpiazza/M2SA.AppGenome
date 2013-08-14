using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace M2SA.AppGenome.Data
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TId"></typeparam>
    public interface IRepository<T, TId>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        T CreateNew();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        T FindById(TId id);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        bool Save(T model);
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        bool Remove(T model);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IList<T> LoadAll();
    }
}
