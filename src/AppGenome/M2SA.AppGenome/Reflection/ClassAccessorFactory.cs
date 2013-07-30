using System;
using System.Collections.Generic;
using System.Text;

namespace M2SA.AppGenome.Reflection
{
    /// <summary>
    /// 
    /// </summary>
    public static class ClassAccessorFactory
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="targetType"></param>
        /// <param name="accessorType"></param>
        /// <returns></returns>
        public static IClassAccessor CreateClassAccessor(Type targetType, AccessorType accessorType)
        {
            IClassAccessor accessor = null;

            switch (accessorType)
            {
                case AccessorType.Reflection:
                    {
                        accessor = new ReflectionClassAccessor(targetType);
                        break;
                    }
                default:
                    {
                        accessor = new EmitClassAccessor(targetType);
                        break;
                    }
            }

            return accessor;
        }
    }
}
