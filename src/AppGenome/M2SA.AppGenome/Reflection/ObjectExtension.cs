using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Web;
using M2SA.AppGenome.Configuration;
using M2SA.AppGenome.Web;

namespace M2SA.AppGenome.Reflection
{
    /// <summary>
    /// 
    /// </summary>
    public static class ObjectExtension
    {
        private static readonly string ObjectTypeName = "type";
        private static readonly string ObjectMapSchemaURI = "http://m2sa.net/Schema/ObjectMap";

        #region dir print

        /// <summary>
        /// 打印对象信息
        /// </summary>
        /// <param name="val"></param>
        public static void Print(this object val)
        {
            var text = val.ToText();
            Console.WriteLine(text);
        }

        #endregion

        #region ToText

        /// <summary>
        /// 
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static string ToText(this object val)
        {
            if (val == null)
                return "[Null]";

            if (val.GetType().IsPrimitiveType())
            {
                string text = null;
                if (val is DateTime)
                    text = ((DateTime)val).ToString("yyyy-MM-dd HH:mm:ss.fff");
                else
                    text = val.ToString();

                return text;
            }
            
            var output = new StringBuilder(256);
            return ToText(val, val.GetType().Name, 0, output).ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static string ToXmlText(this object val)
        {
            var text = val.ToText();
            if (null != text)
            {
                text =
                    text.Replace("<", "&lt;")
                        .Replace(">", "&gt;")
                        .Replace("&", "&amp;")
                        .Replace("'", "&apos;")
                        .Replace("\"", "&quot;");
            }
            return text;
        }

        static StringBuilder ToText(object val, string name, int level, StringBuilder output)
        {
            var pre = new string('\t', level + 1);
            if (val is IList)
            {
                var list = (IList)val;
                output.AppendFormat("{0}{1}:List[{2}]", pre, name, list.Count).AppendLine();
                for (var i = 0; i < list.Count; i++)
                {
                    ToText(list[i], string.Format("{0}.{1}", "item", i), level + 1, output);
                }
            }
            else if (val is IDictionary)
            {
                var map = (IDictionary)val;
                output.AppendFormat("{0}{1}:Map[{2}]", pre, name, map.Count).AppendLine();
                foreach (DictionaryEntry item in map)
                {
                    ToText(item.Value, item.Key.ToString(), level + 1, output);
                }
            }
            else if (val is IConfigNode)
            {
                output.AppendFormat("{0}{1}:IConfigNode", pre, name).AppendLine();
                ToText((IConfigNode)val, level + 1, output);
            }
            else if (val == null || val.GetType().IsPrimitiveType())
            {
                output.AppendFormat("{0}{1}:{2}", pre, name, val.ToText()).AppendLine();
            }
            else if (val is Exception)
            {
                output.AppendFormat("{0}{1}:{2}", pre, name, string.Empty).AppendLine();
                ToText((Exception)val, level + 1, output);
            }
            else if (val is HttpCookie)
            {
                ToText(new HttpCookieInfo((HttpCookie)val), name, level, output);
            }
            else
            {
                var map = val.GetPropertyValues();
                output.AppendFormat("{0}{1}:{2}", pre, name, val.GetType()).AppendLine();
                foreach (var item in map)
                {
                    ToText(item.Value, item.Key, level + 1, output);
                }
            }

            return output;
        }

        static StringBuilder ToText(IConfigNode val, int level, StringBuilder output)
        {            
            var map = val.GetProperties();
            foreach (var item in map)
            {
                ToText(item.Value, item.Key, level + 1, output);
            }
            return output;
        }

        static StringBuilder ToText(Exception ex, int level, StringBuilder output)
        {
            var pre = new string('\t', level + 1);
            output.AppendFormat("{0}{1} : {2}", pre, ex.GetType().FullName, ex.Message).AppendLine();

            if (ex.InnerException != null)
            {
                BuildInnerException(ex.InnerException, level + 1, output);
            }

            if (ex.StackTrace != null)
            {
                output.AppendFormat("{0}Stack trace:", pre).AppendLine();
                output.AppendFormat("{0}{1}", pre, ex.StackTrace.Replace("\r\n", "\r\n" + pre)).AppendLine();
            }            

            return output;
        }

        static StringBuilder ToText(HttpCookie cookie, int level, StringBuilder output)
        {
            var pre = new string('\t', level + 1);
            if (cookie.HasKeys)
            {
                output.AppendFormat("{0}{1} : ", pre, "Values").AppendLine();

                var pre2 = new string('\t', level + 2);
                foreach (DictionaryEntry entry in cookie.Values)
                {
                    output.AppendFormat("{0}{1} : {2}", pre2, entry.Key, entry.Value).AppendLine();
                }
            }
            else
            {
                output.AppendFormat("{0}{1} : {2}", pre, "Value", cookie.Value).AppendLine();
            }

            output.AppendFormat("{0}{1} : {2}", pre, "Domain", cookie.Domain).AppendLine();
            output.AppendFormat("{0}{1} : {2}", pre, "Path", cookie.Path).AppendLine();
            output.AppendFormat("{0}{1} : {2}", pre, "Expires", cookie.Expires).AppendLine();

            return output;
        }

        static StringBuilder BuildInnerException(Exception ex, int level, StringBuilder output)
        {
            var pre = new string('\t', level + 1);
            output.AppendFormat("{0}{1} : {2}", pre, ex.GetType().FullName, ex.Message).AppendLine();
            if (ex.StackTrace != null)
            {
                output.AppendFormat("{0}Stack trace:", pre).AppendLine();
                output.AppendFormat("{0}{1}", pre, ex.StackTrace.Replace("\r\n", "\r\n" + pre)).AppendLine();
            }  

            if (ex.InnerException != null)
            {
                BuildInnerException(ex.InnerException, level, output);
            }
            return output;
        }

        #endregion

        #region ToXml

        /// <summary>
        /// 
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static XmlDocument ToXml(this object val)
        {
            ArgumentAssertion.IsNotNull(val, "val"); ;

            var targetType = val.GetType();

            var output = new XmlDocument();
            output.AppendChild(output.CreateProcessingInstruction("xml", "version=\"1.0\" encoding=\"utf-8\""));

            var rootElement = output.CreateElement("objectMap");
            rootElement.SetAttribute("xmlns:o", ObjectMapSchemaURI);
            output.AppendChild(rootElement);

            var objElement = CreateElement(val, output);
            objElement.SetAttribute(ObjectTypeName, ObjectMapSchemaURI, targetType.GetSimpleQualifiedName());
            rootElement.AppendChild(objElement);
            
            AppendChildNodes(val, objElement);

            return output;
        }       

        static XmlElement CreateElement(object val, XmlDocument xml)
        {
            var element = xml.CreateElement(val.GetType().Name);
            return element;
        }

        static XmlElement CreateElement(object val, string propName, Type propType, XmlDocument xml)
        {
            var element = xml.CreateElement(propName);
            if (val == null)
                return element;

            if (val is HttpCookieInfo)
            {
                element.SetAttribute(ObjectTypeName, ObjectMapSchemaURI, typeof(HttpCookieInfo).GetSimpleQualifiedName());
            }
            else if ((val is Exception))
            {
                element.SetAttribute(ObjectTypeName, ObjectMapSchemaURI, val.GetType().GetSimpleQualifiedName());
            }
            else 
            {
                var targetType = val.GetType();
                if (targetType.IsListType() || targetType.IsDictionaryType() || propType != null && propType.CanCreated() == false)
                    element.SetAttribute(ObjectTypeName, ObjectMapSchemaURI, val.GetType().GetSimpleQualifiedName());
            }
 
            return element;
        }

        static XmlDocument AppendChildNodes(object val, XmlElement target)
        {
            if (val == null)
                return target.OwnerDocument;

            if (val.GetType().IsListType())
            {
                var list = (IList)val;
                for (var i = 0; i < list.Count; i++)
                {
                    var itemElement = CreateElement(list[i], string.Format("{0}.{1}", "item", i), null, target.OwnerDocument);
                    target.AppendChild(itemElement);
                    AppendChildNodes(list[i], itemElement);
                }
            }
            else if (val.GetType().IsDictionaryType())
            {
                var map = (IDictionary)val;
                foreach (DictionaryEntry item in map)
                {
                    var itemElement = CreateElement(item.Value, "item", null, target.OwnerDocument);
                    itemElement.SetAttribute("key", item.Key.ToString());
                    target.AppendChild(itemElement);
                    AppendChildNodes(item.Value, itemElement);
                }
            }
            else if (val.GetType().IsPrimitiveType())
            {
                SetElementValue(target, val);
             }
            else if (val is Exception)
            {
                AppendChildNodes((Exception)val, target);                
            }
            else if (val is HttpCookie)
            {
                AppendChildNodes(new HttpCookieInfo((HttpCookie)val), target);
            }
            else
            {
                var propMap = val.GetType().GetPersistProperties();
                var valueMap = val.GetPropertyValues();
                foreach (var item in valueMap)
                {
                    if (item.Value != null) // 假设对象实例的属性(引用类型)的默认值为空
                    {
                        Type propType = null;
                        if (propMap.ContainsKey(item.Key))
                            propType = propMap[item.Key];

                        AppendProperty(item.Value, item.Key, propType, target);
                    }
                }
            }

            return target.OwnerDocument;
        }

        static XmlDocument AppendChildNodes(Exception ex, XmlElement target)
        {
            var msgElement = CreateElement(ex.Message, "Message", null, target.OwnerDocument);
            SetElementValue(msgElement, ex.Message);
            target.AppendChild(msgElement);

            if (ex.StackTrace != null)
            {
                var stackElement = CreateElement(ex.Message, "StackTrace", null, target.OwnerDocument);
                SetElementValue(stackElement, ex.StackTrace);
                target.AppendChild(stackElement);
            }
            if (ex.InnerException != null)
            {
                var innerElement = CreateElement(ex.InnerException, "InnerException", null, target.OwnerDocument);
                target.AppendChild(innerElement);

                AppendChildNodes(ex.InnerException, innerElement);
            }
            return target.OwnerDocument;
        }        

        static void AppendProperty(object val, string propName, Type propType, XmlElement target)
        {
            if (val == null)
                return;

            var appendAttribute = false;
            var enableCompact = false;

            if (enableCompact && val.GetType().IsPrimitiveType())
            {
                var text = val.ToText();
                if (text.IndexOf("\n") == -1)
                {
                    target.SetAttribute(propName, text);
                    appendAttribute = true;
                }
            }

            if (enableCompact == false || appendAttribute == false)
            {
                var itemElement = CreateElement(val, propName, propType, target.OwnerDocument);
                target.AppendChild(itemElement);

                AppendChildNodes(val, itemElement);
            }
        }

        static void SetElementValue(XmlNode node, object val)
        {
            if (val == null)
                return;

            string text = val.ToText();
            if (text.IndexOf("\n") != -1)
            {
                var section = node.OwnerDocument.CreateCDataSection(text);
                node.AppendChild(section);
            }
            else
            {
                node.InnerText = text;
            }       
        }

        #endregion

        #region Property Access

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static IDictionary<string, object> GetPropertyValues(this object target)
        {
            ArgumentAssertion.IsNotNull(target, "target");

            var targetType = target.GetType();
            var accessor = ClassAccessorRepository.GetClassAccessor(targetType);
            var targetProperties = accessor.PropertyAccessores;

            var propertyValues = new Dictionary<string, object>(targetProperties.Count);

            foreach (var pair in targetProperties)
            {
                var prop = pair.Value;
                var propName = prop.Name;
                var propValue = accessor.GetValue(target, propName);
                propertyValues.Add(propName, propValue);
            }

            return propertyValues;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="propertyNames"></param>
        /// <returns></returns>
        public static IDictionary<string, object> GetPropertyValues(this object target, IList<string> propertyNames)
        {
            ArgumentAssertion.IsNotNull(target, "target");
            ArgumentAssertion.IsNotNull(propertyNames, "propertyNames");

            var targetType = target.GetType();
            var accessor = ClassAccessorRepository.GetClassAccessor(targetType);

            var propertyValues = new Dictionary<string, object>(propertyNames.Count);

            foreach (var propName in propertyNames)
            {
                if (accessor.PropertyAccessores.ContainsKey(propName.ToLower()))
                {
                    var propValue = accessor.GetValue(target, propName);
                    propertyValues.Add(propName, propValue);
                }
            }

            return propertyValues;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="propertyValues"></param>
        public static void SetPropertyValues(this object target, IDictionary<string, object> propertyValues)
        {
            SetPropertyValues(target, propertyValues, AccessorType.Reflection);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="propertyValues"></param>
        /// <param name="accessorType"></param>
        public static void SetPropertyValues(this object target, IDictionary<string, object> propertyValues,
            AccessorType accessorType)
        {
            ArgumentAssertion.IsNotNull(propertyValues, "propertyValues");

            var accessor = ClassAccessorRepository.GetClassAccessor(target, accessorType);

            foreach (var pair in propertyValues)
            {
                accessor.SetValue(target, pair.Key, pair.Value);
            }
        }

        #endregion

        #region Convert

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T Convert<T>(this object value)
        {
            var targetType = typeof(T);
            var target = value.Convert(targetType);
            return (T)target;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <returns></returns>
        public static object Convert(this object value, Type targetType)
        {
            ArgumentAssertion.IsNotNull(value, "value");
            ArgumentAssertion.IsNotNull(targetType, "targetType");

            object target = null;
            if (targetType.IsEnum)
            {
                target = Enum.Parse(targetType, value.ToString(), true);
            }
            if (targetType.Equals(typeof(TimeSpan)))
            {
                target = TimeSpan.Parse(value.ToString());
            }
            else
            {
                var code = Type.GetTypeCode(targetType);
                switch (code)
                {
                    case TypeCode.Boolean:
                        target = System.Convert.ToBoolean(value);
                        break;
                    case TypeCode.Byte:
                        target = System.Convert.ToByte(value);
                        break;
                    case TypeCode.Char:
                        target = System.Convert.ToChar(value);
                        break;
                    case TypeCode.DateTime:
                        target = System.Convert.ToDateTime(value);
                        break;
                    case TypeCode.Decimal:
                        target = System.Convert.ToDecimal(value);
                        break;
                    case TypeCode.Double:
                        target = System.Convert.ToDouble(value);
                        break;
                    case TypeCode.Int16:
                        target = System.Convert.ToInt16(value);
                        break;
                    case TypeCode.Int32:
                        target = System.Convert.ToInt32(value);
                        break;
                    case TypeCode.Int64:
                        target = System.Convert.ToInt64(value);
                        break;
                    case TypeCode.SByte:
                        target = System.Convert.ToSByte(value);
                        break;
                    case TypeCode.Single:
                        target = System.Convert.ToSingle(value);
                        break;
                    case TypeCode.UInt16:
                        target = System.Convert.ToUInt16(value);
                        break;
                    case TypeCode.UInt32:
                        target = System.Convert.ToUInt32(value);
                        break;
                    case TypeCode.UInt64:
                        target = System.Convert.ToUInt64(value);
                        break;
                    case TypeCode.String:
                        target = System.Convert.ToString(value);
                        break;
                    default:
                        target = value;
                        break;
                }
            }

            return target;
        }

        #endregion
    }
}
