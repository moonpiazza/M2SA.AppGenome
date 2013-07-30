using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace M2SA.AppGenome.Configuration
{
    /// <summary>
    /// 
    /// </summary>
    public interface IResolveObject
    {
        /// <summary>
        /// 根据配置进行初始化
        /// </summary>
        /// <param name="config"></param>
        void Initialize(IConfigNode config);
    }
}
