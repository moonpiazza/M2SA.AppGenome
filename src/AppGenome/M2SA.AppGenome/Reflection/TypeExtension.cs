using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace M2SA.AppGenome.Reflection
{
    /// <summary>
    /// 
    /// </summary>
    public static class TypeExtension
    {
        /// <summary>
        /// 是否是基础类型
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static bool IsPrimitiveType(this Type target)
        {
            ArgumentAssertion.IsNotNull(target, "target");
            return ((target.IsValueType && Type.GetTypeCode(target) != TypeCode.Object) || target.Equals(typeof(string))|| target.Equals(typeof(TimeSpan)));
        }

        /// <summary>
        /// 是否是IList
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static bool IsListType(this Type target)
        {
            ArgumentAssertion.IsNotNull(target, "target");
            var result = false;
            if (target.Equals(typeof(IList)) || target.GetInterface("System.Collections.IList") != null)
            {
                result = true;
            }
            else if (target.IsGenericType)
            {
                var args = target.GetGenericArguments();
                if (args.Length == 1)
                {
                    var genericType = typeof(IList<>).MakeGenericType(args);
                    if (genericType.Equals(target) || target.GetInterface("System.Collections.Generic.IList`1") != null)
                    {
                        result = true;
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// 是否是IList
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static bool IsDictionaryType(this Type target)
        {
            ArgumentAssertion.IsNotNull(target, "target");

            var result = false;
            if (target.Equals(typeof(IDictionary)) || target.GetInterface("System.Collections.IDictionary") != null)
            {
                result = true;
            }
            else if (target.IsGenericType)
            {
                var args = target.GetGenericArguments();
                if (args.Length == 2)
                {
                    var genericType = typeof(IDictionary<,>).MakeGenericType(args);
                    if (genericType.Equals(target) || target.GetInterface("System.Collections.Generic.IDictionary`2") != null)
                    {
                        result = true;
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static Type GetMapType(this Type target)
        {
            ArgumentAssertion.IsNotNull(target, "target");

            Type result = null;
            var aliasKeys = new string[] { target.Name, target.FullName };

            if (target.IsInterface && target.Name.StartsWith("I"))
            {
                aliasKeys = new string[] { target.Name, target.Name.Substring(1), target.FullName };
            }

            aliasKeys.FirstOrDefault<string>(aliasKey => 
                {
                    result = GetMapTypeByAlias(aliasKey);
                    return result != null;
                });

            if (null == result && target.IsInterface && target.Name.StartsWith("I"))
            {
                var typeName = target.Name.Substring(1);
                var implementTypes = new string[] {                      
                    string.Format("{0}.{1},{2}", target.Namespace, typeName, target.Assembly.FullName.Split(',')[0]),
                    string.Format("{0}.{1},{2}", target.Namespace, typeName, target.Namespace),
                };

                implementTypes.FirstOrDefault<string>(typeFullName => {
                    result = Type.GetType(typeFullName);
                        return result != null;
                    });
            }

            if (null == result)
            {
                result = target;
            }
            else if (result.IsGenericType && target.IsGenericType && result.IsGenericTypeDefinition)
            {
                var args = target.GetGenericArguments();
                result = result.MakeGenericType(args);
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="typeAlias"></param>
        /// <returns></returns>
        public static Type GetMapType(string typeAlias)
        {
            Type result = GetMapTypeByAlias(typeAlias);
            if (null == result)
                result = Type.GetType(typeAlias);                
            return result;
        }

        static Type GetMapTypeByAlias(string typeAlias)
        {
            Type result = null;
            if (AppInstance.Config == null) return null;
            var aliasKeys = new List<string>(AppInstance.Config.Modules.Count + 1);
            aliasKeys.Add(typeAlias);
            foreach (var moduleKey in AppInstance.Config.Modules.Keys)
            {
                aliasKeys.Add(string.Format("{0}.{1}", moduleKey, typeAlias));
            }

            aliasKeys.FirstOrDefault<string>(aliasKey => 
                {
                    if (AppInstance.Config.TypeAliases.ContainsKey(aliasKey))
                        result = Type.GetType(AppInstance.Config.TypeAliases[aliasKey]);
                    return result != null;
                });
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string GetSimpleQualifiedName(this Type type)
        {
            ArgumentAssertion.IsNotNull(type, "type");

            var assemblyInfo = type.Assembly.FullName.Split(',');
            var assemblyName = assemblyInfo[0];
            if (assemblyName.StartsWith("mscorlib"))
            {
                assemblyName = null;
            }

            StringBuilder nameBuilder = new StringBuilder(256);
            if (false == type.IsGenericType)
            {
                nameBuilder.Append(type.FullName);
            }
            else
            {
                nameBuilder.AppendFormat("{0}.{1}[", type.Namespace, type.Name);
                var args = type.GetGenericArguments();
                var isFisrt = true;
                foreach (var arg in args)
                {
                    if (isFisrt)
                    {
                        nameBuilder.AppendFormat("[{0}]", arg.GetSimpleQualifiedName());
                        isFisrt = false;
                    }
                    else
                    {
                        nameBuilder.AppendFormat(",[{0}]", arg.GetSimpleQualifiedName());
                    }
                }
                nameBuilder.Append("]");
            }
            if (false == string.IsNullOrEmpty(assemblyName))
            {
                nameBuilder.AppendFormat(",{0}", assemblyName);
            }

            return nameBuilder.ToString();
        }        

        /// <summary>
        /// 类型是否可以创建实例
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool CanCreated(this Type type)
        {
            ArgumentAssertion.IsNotNull(type, "type");
            return (type.IsAbstract == false && type.IsInterface == false);
        }

        /// <summary>
        /// 获取指定类型的可以被持久化的属性列表
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static IDictionary<string, Type> GetPersistProperties(this Type type)
        {
            var accessor = ClassAccessorRepository.GetClassAccessor(type);
            return GetPersistProperties(type, accessor);
        }

        /// <summary>
        /// 获取指定类型的可以被持久化的属性列表
        /// </summary>
        /// <param name="type"></param>
        /// <param name="accessorType"></param>
        /// <returns></returns>
        public static IDictionary<string, Type> GetPersistProperties(this Type type, AccessorType accessorType)
        {
            var accessor = ClassAccessorRepository.GetClassAccessor(type, accessorType);
            return GetPersistProperties(type, accessor);
        }

        /// <summary>
        /// 获取指定类型的可以被持久化的属性列表
        /// </summary>
        /// <param name="type"></param>
        /// <param name="accessor"></param>
        /// <returns></returns>
        static IDictionary<string, Type> GetPersistProperties(this Type type, IClassAccessor accessor)
        {
            var targetProperties = accessor.PropertyAccessores;
            var propertyTypes = new Dictionary<string, Type>(targetProperties.Count);

            foreach (var pair in targetProperties)
            {
                var prop = pair.Value;
                var propName = prop.Name;
                var propType = prop.PropertyType;

                if (prop.CanRead && prop.CanWrite && prop.NonSerialized == false)
                {
                    propertyTypes.Add(propName, propType);
                }
            }

            return propertyTypes;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public static Type GetPropertyType(this Type type, string propertyName)
        {
            var propertyTypes = GetPersistProperties(type);
            if (propertyTypes.ContainsKey(propertyName))
            {
                return propertyTypes[propertyName];
            }
            return null;
        }
    }
}
