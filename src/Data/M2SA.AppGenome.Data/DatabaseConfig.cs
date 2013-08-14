using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace M2SA.AppGenome.Data
{
    /// <summary>
    /// 
    /// </summary>
    public class DatabaseConfig
    {
        /// <summary>
        /// 
        /// </summary>
        public string ConfigName
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public string ConnectionString
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public DatabaseType DBType
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public string ProviderName
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public DatabaseConfig()
        {
        }
    }
}
