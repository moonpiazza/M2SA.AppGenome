using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using M2SA.AppGenome.Configuration;

namespace M2SA.AppGenome.Logging
{
    /// <summary>
    /// 
    /// </summary>
    public class SystemLogger : ILog
    {
        #region Static members

        /// <summary>
        /// 
        /// </summary>
        private static readonly IDictionary<LogLevel, ConsoleColor> ConsoleColors = new Dictionary<LogLevel, ConsoleColor>()
        {
            {LogLevel.Debug, ConsoleColor.DarkGray},
            {LogLevel.Trace, ConsoleColor.DarkCyan},
            {LogLevel.Info, ConsoleColor.White},
            {LogLevel.Warn, ConsoleColor.Yellow},
            {LogLevel.Error, ConsoleColor.DarkRed},
            {LogLevel.Fatal, ConsoleColor.DarkMagenta}
        };

        /// <summary>
        /// 
        /// </summary>
        /// <param name="level"></param>
        /// <param name="format"></param>
        /// <param name="args"></param>
        public static void WriteLine(LogLevel level, string format, params object[] args)
        {
            var color = ConsoleColor.White;
            if (ConsoleColors.ContainsKey(level))
                color = ConsoleColors[level];

            if (level >= LogLevel.Warn)
                Console.BackgroundColor = ConsoleColor.DarkGray;
            Console.ForegroundColor = color;
            Console.WriteLine(format, args);
            Console.ResetColor();
        }

        #endregion //Static members

        private LogLevel minLevel;
        private LogLevel maxLevel;

        /// <summary>
        /// 
        /// </summary>
        public SystemLogger()
        {
            this.Initialize(null);
        }

        #region ILog 成员

        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="level"></param>
        /// <param name="msg"></param>
        /// <param name="exception"></param>
        public void WriteMessage(LogLevel level, object msg, Exception exception)
        {
            if (IsWrite(level) == false)
                return;

            if (null == exception)
            {
                WriteLine(level, "{0}:{1}", level, msg);
            }
            else
            {
                WriteLine(level, "[{0}]{1}", level, exception.GetType().FullName, exception.Message);
                WriteLine(level, "\t{0}", exception.StackTrace);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public void Debug(object message)
        {
            this.WriteMessage(LogLevel.Debug, message, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        public void Debug(string format, params object[] args)
        {
            this.WriteMessage(LogLevel.Debug, string.Format(format, args), null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public void Trace(object message)
        {
            this.WriteMessage(LogLevel.Trace, message, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        public void Trace(string format, params object[] args)
        {
            this.WriteMessage(LogLevel.Trace, string.Format(format, args), null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public void Info(object message)
        {
            this.WriteMessage(LogLevel.Info, message, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        public void Info(string format, params object[] args)
        {
            this.WriteMessage(LogLevel.Info, string.Format(format, args), null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public void Warn(object message)
        {
            this.WriteMessage(LogLevel.Warn, message, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        public void Warn(string format, params object[] args)
        {
            this.WriteMessage(LogLevel.Warn, string.Format(format, args), null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="exception"></param>
        /// <param name="message"></param>
        public void Warn(Exception exception, object message)
        {
            this.WriteMessage(LogLevel.Warn, message, exception);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public void Error(object message)
        {
            this.WriteMessage(LogLevel.Error, message, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        public void Error(string format, params object[] args)
        {
            this.WriteMessage(LogLevel.Error, string.Format(format, args), null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="exception"></param>
        /// <param name="message"></param>
        public void Error(Exception exception, object message)
        {
            this.WriteMessage(LogLevel.Error, message, exception);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public void Fatal(object message)
        {
            this.WriteMessage(LogLevel.Fatal, message, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        public void Fatal(string format, params object[] args)
        {
            this.WriteMessage(LogLevel.Fatal, string.Format(format, args), null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="exception"></param>
        /// <param name="message"></param>
        public void Fatal(Exception exception, object message)
        {
            this.WriteMessage(LogLevel.Fatal, message, exception);
        }

        #endregion        

        /// <summary>
        /// 
        /// </summary>
        /// <param name="config"></param>
        public void Initialize(IConfigNode config)
        {
            this.minLevel = LogLevel.Info;
            this.maxLevel = LogLevel.Off;

            if (AppInstance.Config.Debug)
            {
                this.minLevel = LogLevel.All;
            }
        }


        bool IsWrite(LogLevel level)
        {
            return (level >= this.minLevel && level <= this.maxLevel);
        }
    }
}
