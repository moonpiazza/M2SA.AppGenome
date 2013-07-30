using System;
using System.Collections.Generic;
using System.Text;

namespace M2SA.AppGenome.Reflection
{
    /// <summary>
    /// 
    /// </summary>
    public static class ClassAccessorRepository
    {
        private static object syncObject = new object();
        private static IDictionary<string, IClassAccessor> ClassAccessores = new Dictionary<string, IClassAccessor>(16);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static IClassAccessor GetClassAccessor(object target)
        {
            if (null == target)
                throw new ArgumentNullException("target");
            var type = target.GetType();
            return GetClassAccessor(type);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="accessorType"></param>
        /// <returns></returns>
        public static IClassAccessor GetClassAccessor(object target, AccessorType accessorType)
        {
            if (null == target)
                throw new ArgumentNullException("target");
            var type = target.GetType();
            return GetClassAccessor(type, accessorType);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="targetType"></param>
        /// <returns></returns>
        public static IClassAccessor GetClassAccessor(Type targetType)
        {
            return GetClassAccessor(targetType, AccessorType.Emit);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="targetType"></param>
        /// <param name="accessorType"></param>
        /// <returns></returns>
        public static IClassAccessor GetClassAccessor(Type targetType, AccessorType accessorType)
        {
            if (null == targetType)
                throw new ArgumentNullException("targetType");

            IClassAccessor accessor = null;
            var typeKey = string.Format("{0}.{1}", accessorType, targetType.FullName);
            if (ClassAccessores.ContainsKey(typeKey))
            {
                accessor = ClassAccessores[typeKey];
            }
            else
            {
                lock (syncObject)
                {
                    if (ClassAccessores.ContainsKey(typeKey))
                    {
                        accessor = ClassAccessores[typeKey];
                    }
                    else
                    {
                        accessor = ClassAccessorFactory.CreateClassAccessor(targetType, accessorType);
                        ClassAccessores.Add(typeKey, accessor);
                    }
                }
            }
            return accessor;
        }
    }
}
