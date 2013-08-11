using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace M2SA.AppGenome.Cache
{
    /// <summary>
    /// 
    /// </summary>
    public static class ArgumentAssertion
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="argumentValue"></param>
        /// <param name="argumentName"></param>
        public static void IsNotNull(object argumentValue, string argumentName)
        {
            if (argumentValue == null)
                throw new ArgumentNullException(argumentName);
        }
    }
}
