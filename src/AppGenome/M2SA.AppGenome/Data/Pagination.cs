using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace M2SA.AppGenome.Data
{
    /// <summary>
    /// 
    /// </summary>
    public class Pagination
    {
        private int pageIndex = 1;

        /// <summary>
        /// 
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int PageIndex 
        { 
            get { return this.pageIndex; }
            set
            {
                if (value < 1) 
                    throw new ArgumentOutOfRangeException("value", "value be more than or equal to 1.");
                this.pageIndex = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int LimitQueryCount { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Pagination()
        {
            this.PageSize = 10;
        }
    }
}
