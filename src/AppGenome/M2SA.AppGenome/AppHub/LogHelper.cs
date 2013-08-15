using M2SA.AppGenome.Logging;

namespace M2SA.AppGenome.AppHub
{
    /// <summary>
    /// 
    /// </summary>
    public class LogHelper
    {
        /// <summary>
        /// 
        /// </summary>
        public static readonly string LoggerName = "HostLogger";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="container"></param>
        /// <param name="info"></param>
        /// <param name="args"></param>
        static void Info(object appId, string container, string info, params object[] args)
        {
            info = string.Format("[{0} ]{1}", container, info);

            var logEntry = new LogEntry() {
                SessionId = appId.ToString(),
                Message = string.Format(info, args)
            };

            LogManager.GetLogger(LogHelper.LoggerName).Info(logEntry);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="info"></param>
        /// <param name="args"></param>
        public static void AppInfo(object appId, string info, params object[] args)
        {
            Info(appId, "App", info, args);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="info"></param>
        /// <param name="args"></param>
        public static void HostInfo(object appId, string info, params object[] args)
        {
            Info(appId, "Host", info, args);
        }
    }
}