using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using M2SA.AppGenome.Configuration;
using M2SA.AppGenome.Reflection;

namespace M2SA.AppGenome.Configuration
{
    /// <summary>
    /// 
    /// </summary>
    public static class ObjectBuilder
    {
        /// <summary>
        /// 创建对象实例
        /// </summary>
        /// <param name="typeAlias"></param>
        /// <returns></returns>
        public static object BuildObject(string typeAlias)
        {
            return BuildObject(null, typeAlias);
        }

        /// <summary>
        /// 创建对象实例
        /// </summary>
        /// <param name="targetType"></param>
        /// <returns></returns>
        public static object BuildObject(this Type targetType)
        {
            return BuildObject(targetType, (string)null);
        }

        /// <summary>
        /// 创建对象实例
        /// </summary>
        /// <param name="targetType"></param>
        /// <param name="typeAlias"></param>
        /// <returns></returns>
        public static object BuildObject(this Type targetType, string typeAlias)
        {
            var objType = GetMapType(targetType, typeAlias);
            if (null == objType)
            {
                throw new ArgumentNullException("targetType", string.Format("Cannot find type : {0}!", typeAlias));
            }
            if (TypeExtension.CanCreated(objType) == false)
            {
                throw new NotImplementedException(string.Format("{0} cannot be created", objType));
            }

            try
            {
                var obj = Activator.CreateInstance(objType, true);

                return obj;
            }
            catch (Exception ex)
            {
                throw new ArgumentException(string.Format("{0} cannot be created : {1}", objType, ex.Message)); 
            }
        }

        /// <summary>
        /// 设置对象属性
        /// </summary>
        /// <param name="source"></param>
        /// <param name="configNode"></param>
        /// <returns></returns>
        public static IResolveObject DeserializeObject(this IResolveObject source, IConfigNode configNode)
        {
            return (IResolveObject)DeserializeObject((object)source, configNode);
        }

        private static object BuildObject(Type targetType, IConfigNode configNode)
        {
            ArgumentAssertion.IsNotNull(configNode, "configNode");

            var objTypeDefine = configNode.MetaType;
            var obj = BuildObject(targetType, objTypeDefine);

            if (obj is IResolveObject)
                (obj as IResolveObject).Initialize(configNode);
            else
                obj = DeserializeObject(obj, configNode);

            return obj;
        }

        private static object DeserializeObject(object target, IConfigNode configNode)
        {
            var targetType = target.GetType();
            var propertyValues = configNode.GetProperties();
            var accessor = ClassAccessorRepository.GetClassAccessor(target, AccessorType.Reflection);
            var persistProperties = targetType.GetPersistProperties(AccessorType.Reflection);
            var targetProperties = new Dictionary<string, Type>(persistProperties.Count);
            foreach (var item in persistProperties)
            {
                targetProperties.Add(item.Key.ToLower(), item.Value);
            }

            foreach (var pair in propertyValues)
            {
                var pName = pair.Key;
                var pValue = pair.Value;

                var propertyName = string.Empty;
                var pType = TryGetPropertyType(targetProperties, pName, out propertyName);
                if (pType == null)
                {
                    continue;
                }

                var targetValue = accessor.GetValue(target, propertyName);

                if (pType.IsListType())
                {
                    var targetList = targetValue as IList;
                    if (targetList == null)
                    {
                        targetList = (IList)pType.BuildObject();
                        accessor.SetValue(target, propertyName, targetList);
                    }

                    IList valList = null;
                    if (pValue is IList)
                    {
                        valList = (IList)pValue;
                    }
                    else
                    {
                        valList = new List<object>(1) { pValue };
                    }
                    DeserializeList(pType, targetList, valList);
                }
                else if (pType.IsDictionaryType())
                {
                    var targetMap = targetValue as IDictionary;
                    if (targetMap == null)
                    {
                        targetMap = (IDictionary)pType.BuildObject();
                        accessor.SetValue(target, propertyName, targetMap);
                    }
                    var valMap = configNode.GetNodeMap(pName);
                    DeserializeDictionary(pType, targetMap, valMap);
                }             
                else
                {
                    if (pValue is IConfigNode)
                    {
                        var val = BuildObject(pType, (IConfigNode)pValue);
                        accessor.SetValue(target, propertyName, val);
                    }
                    else 
                    {
                        var val = DeserializePrimitiveValue(pValue, pType);
                        accessor.SetValue(target, propertyName, val);
                    }
                }               
            }

            return target;
        }

        private static void DeserializeList(Type pType, IList targetList, IList values)
        {
            Type paramType = null;
            if (pType.IsGenericType)
            {
                var args = pType.GetGenericArguments();
                paramType = args[0];
            }

            foreach (var item in values)
            {
                if (item is IConfigNode)
                {
                    var tempNode = item as IConfigNode;
                    object val = null;
                    if (paramType.IsPrimitiveType())
                    {
                        var tempValues = tempNode.GetProperties();
                        foreach (var tempProp in tempValues)
                        {
                            val = DeserializePrimitiveValue(tempProp.Value, paramType);
                        }
                    }
                    else
                    {
                        val = BuildObject(paramType, tempNode);
                    }
                    targetList.Add(val);
                }
                else
                {
                    targetList.Add(item);
                }
            }
        }

        static Type TryGetPropertyType(IDictionary<string, Type> targetProperties, string pName, out string targetName)
        {
            Type result = null;
            targetName = pName;
            if (targetProperties.ContainsKey(targetName))
            {
                result = targetProperties[targetName];
            }
            if (result == null)
            {
                targetName = string.Format("{0}list", pName);
                if (targetProperties.ContainsKey(targetName))
                {
                    result = targetProperties[targetName];
                }
            }
            if (result == null)
            {
                targetName = string.Format("{0}map", pName);
                if (targetProperties.ContainsKey(targetName))
                {
                    result = targetProperties[targetName];
                }
            }
            if (result == null)
            {
                targetName = pName.TranslateToPlural();
                if (targetProperties.ContainsKey(targetName))
                {
                    result = targetProperties[targetName];
                }
            }

            return result;
        }

        static void DeserializeDictionary(Type targetType, IDictionary target, IDictionary<string, IConfigNode> valMap)
        {
            var targetMap = target as IDictionary;
            if (targetMap != null)
            {
                Type pairValueType = null;
                if (targetType.IsGenericType)
                {
                    var args = targetType.GetGenericArguments();
                    pairValueType = args[1];
                }

                foreach (var item in valMap)
                {
                    if (item.Value is IConfigNode)
                    {
                        var tempNode = item.Value as IConfigNode;
                        object val = null;
                        if (pairValueType.IsPrimitiveType())
                        {
                            string itemKeyNodeName = AppConfig.SrongNameSequence.FirstOrDefault<string>(
                                name => tempNode.GetProperty<string>(name) == item.Key);

                            var tempValues = tempNode.GetProperties();
                            foreach (var tempProp in tempValues)
                            {
                                if (tempProp.Key != itemKeyNodeName)
                                {
                                    val = DeserializePrimitiveValue(tempProp.Value, targetType);
                                }
                            }
                        }
                        else
                        {
                            val = BuildObject(pairValueType, tempNode);
                        }
                        targetMap[item.Key] = val;
                    }
                    else
                    {
                        targetMap[item.Key] = item.Value;
                    }
                }
            }
        }

        static Type GetMapType(Type source, string typeAlias)
        {
            var result = source;
            if (string.IsNullOrEmpty(typeAlias))
            {
                result = source.GetMapType();
            }
            else
            {
                result = Type.GetType(typeAlias);
                if (null == result)
                {
                    var aliasKeys = new string[] { string.Format("{0}.{1}", source.Name, typeAlias), typeAlias };

                    if (source.IsInterface && source.Name.StartsWith("I"))
                    {
                        aliasKeys = new string[] { string.Format("{0}.{1}", source.Name, typeAlias)
                            , string.Format("{0}.{1}", source.Name.Substring(1), typeAlias), typeAlias };
                    }
                    aliasKeys.FirstOrDefault<string>(aliasKey =>
                    {
                        result = TypeExtension.GetMapType(aliasKey);
                        return result != null;
                    });
                }
            }

            return result;
        }

        static object DeserializePrimitiveValue(object value, Type t)
        {
            if (t.IsEnum)
            {
                return Enum.Parse(t, value.ToString(), true);
            }
            if (t.Equals(typeof(TimeSpan)))
            {
                return TimeSpan.Parse(value.ToString());
            }

            var code = Type.GetTypeCode(t);
            switch (code)
            {
                case TypeCode.Boolean:
                    return Convert.ToBoolean(value);
                case TypeCode.Byte:
                    return Convert.ToByte(value);
                case TypeCode.Char:
                    return Convert.ToChar(value);
                case TypeCode.DateTime:
                    return Convert.ToDateTime(value);
                case TypeCode.Decimal:
                    return Convert.ToDecimal(value);
                case TypeCode.Double:
                    return Convert.ToDouble(value);
                case TypeCode.Int16:
                    return Convert.ToInt16(value);
                case TypeCode.Int32:
                    return Convert.ToInt32(value);
                case TypeCode.Int64:
                    return Convert.ToInt64(value);
                case TypeCode.SByte:
                    return Convert.ToSByte(value);
                case TypeCode.Single:
                    return Convert.ToSingle(value);
                case TypeCode.String:
                    return value;
                case TypeCode.UInt16:
                    return Convert.ToUInt16(value);
                case TypeCode.UInt32:
                    return Convert.ToUInt32(value);
                case TypeCode.UInt64:
                    return Convert.ToUInt64(value);

                default:
                    {
                        object obj = value;

                        //if (configNode.ChildNodes.Count > 0)
                        //{
                        //    obj = Activator.CreateInstance(t);
                        //    this.DeserializeStructFields(obj, configNode);
                        //}

                        return obj;
                    }
            }
        }
    }
}