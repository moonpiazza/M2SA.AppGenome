using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace M2SA.AppGenome.Data
{
    /// <summary>
    /// 
    /// </summary>
    internal static class ArgumentAssertion
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="argumentValue"></param>
        /// <param name="argumentName"></param>
        internal static void IsNotNull(object argumentValue, string argumentName)
        {
            if (argumentValue == null) 
                throw new ArgumentNullException(argumentName);
        }
    }
}
