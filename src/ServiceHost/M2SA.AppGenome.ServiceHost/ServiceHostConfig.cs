using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using System.IO;
using M2SA.AppGenome.Configuration;

namespace M2SA.AppGenome.ServiceHost
{
    /// <summary>
    /// 
    /// </summary>
    public class ServiceHostConfig : ResolveObjectBase
    {
        /// <summary>
        /// 
        /// </summary>
        public class HostConfig
        {
            private string runDirectory = string.Empty;
            private string sourceDirectory = string.Empty;

            /// <summary>
            /// 
            /// </summary>
            public string RunDirectory
            {
                get
                {
                    return this.runDirectory;
                }
                set
                {
                    this.runDirectory = GetFullPath(value);
                }
            }

            /// <summary>
            /// 
            /// </summary>
            public string SourceDirectory
            {
                get
                {
                    return this.sourceDirectory;
                }
                set
                {
                    this.sourceDirectory = GetFullPath(value);
                }
            }

            /// <summary>
            /// 
            /// </summary>
            public bool RunInService { get; set; }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="dir"></param>
            /// <returns></returns>
            private static string GetFullPath(string dir)
            {
                if (dir.Contains(@":\\"))
                    return dir;
                else
                    return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, dir);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public class RunConfig
        {
            /// <summary>
            /// 
            /// </summary>
            public string ExeFile { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public bool Debug { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public bool RenameForHost { get; set; }
        }

        /// <summary>
        /// 
        /// </summary>
        public HostConfig Host { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public RunConfig App { get; set; }

        private ServiceHostConfig()
        {
        }
    }
}
