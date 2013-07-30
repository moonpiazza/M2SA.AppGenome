using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace M2SA.AppGenome.Logging
{
    /// <summary>
    /// 
    /// </summary>
    public interface ILogFactory
    {
        /// <summary>
        /// 获取ILog的默认实例
        /// </summary>
        /// <returns></returns>
        ILog GetLogger();

        /// <summary>
        /// 获取ILog的指定类别的实例
        /// </summary>
        /// <param name="categoryName"></param>
        /// <returns></returns>
        ILog GetLogger(string categoryName);

        /// <summary>
        /// 获取ILog的指定会话标识的默认实例
        /// </summary>
        /// <param name="sessionId"></param>
        /// <returns></returns>
        ILog GetSessionLogger(string sessionId);

        /// <summary>
        /// 获取ILog的指定类别和会话标识的实例
        /// </summary>
        /// <param name="categoryName"></param>
        /// <param name="sessionId"></param>
        /// <returns></returns>
        ILog GetSessionLogger(string categoryName, string sessionId);
    }
}
