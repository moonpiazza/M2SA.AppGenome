using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using M2SA.AppGenome.Logging;
using M2SA.AppGenome.Reflection;

namespace M2SA.AppGenome.Logging
{
    /// <summary>
    /// 
    /// </summary>
    public static class LogManager
    {
        /// <summary>
        /// 获取ILog的默认实例
        /// </summary>
        /// <returns></returns>
        public static ILog GetLogger()
        {
            if (typeof(ILogFactory).GetMapType().CanCreated() == false)
                return GetSystemLogger();
            return ObjectIOCFactory.GetSingleton<ILogFactory>().GetLogger();
        }

        /// <summary>
        /// 获取ILog的指定类别的实例
        /// </summary>
        /// <param name="categoryName"></param>
        /// <returns></returns>
        public static ILog GetLogger(string categoryName)
        {
            if (typeof(ILogFactory).GetMapType().CanCreated() == false)
                return GetSystemLogger();
            return ObjectIOCFactory.GetSingleton<ILogFactory>().GetLogger(categoryName);
        }

        /// <summary>
        /// 获取ILog的指定会话标识的默认实例
        /// </summary>
        /// <param name="sessionId"></param>
        /// <returns></returns>
        public static ILog GetSessionLogger(string sessionId)
        {
            return ObjectIOCFactory.GetSingleton<ILogFactory>().GetSessionLogger(sessionId);
        }

        /// <summary>
        /// 获取ILog的指定类别和会话标识的实例
        /// </summary>
        /// <param name="categoryName"></param>
        /// <param name="sessionId"></param>
        /// <returns></returns>
        public static ILog GetSessionLogger(string categoryName, string sessionId)
        {
            return ObjectIOCFactory.GetSingleton<ILogFactory>().GetSessionLogger(categoryName, sessionId);
        }

        static ILog GetSystemLogger()
        {
            var log = ObjectIOCFactory.GetSingleton<SystemLogger>();
            log.Warn("not create a LogFactory");
            return log;
        }
    }
}
