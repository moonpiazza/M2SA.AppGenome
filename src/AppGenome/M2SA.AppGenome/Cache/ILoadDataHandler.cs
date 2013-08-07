using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace M2SA.AppGenome.Cache
{

    /// <summary>
    /// 缓存项操作接口
    /// </summary>
    public interface ILoadDataHandler
    {
        /// <summary>
        /// 根据Key从数据源获取数据
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        object LoadData(string key);
    }
}
