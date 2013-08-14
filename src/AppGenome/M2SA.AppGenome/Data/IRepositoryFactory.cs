using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace M2SA.AppGenome.Data
{
    /// <summary>
    /// 
    /// </summary>
    public interface IRepositoryFactory
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        TRepository GetRepository<TRepository>();
    }
}
