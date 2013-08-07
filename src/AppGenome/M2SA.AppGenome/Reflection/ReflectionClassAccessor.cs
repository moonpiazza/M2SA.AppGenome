using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Reflection;
using System.Diagnostics;
using M2SA.AppGenome.Logging;

namespace M2SA.AppGenome.Reflection
{
    internal class ReflectionClassAccessor : IClassAccessor
    {
        private Type m_TargetType;
        
        /// <summary>
        /// 类型的实例属性
        /// </summary>
        public IDictionary<string, IPropertyAccessorInfo> PropertyAccessores
        {
            get;
            set;
        }

        /// <summary>
        /// Creates a new property accessor.
        /// </summary>
        /// <param name="targetType">Target object type.</param>
        public ReflectionClassAccessor(Type targetType)
        {
            this.m_TargetType = targetType;
            this.PropertyAccessores = new Dictionary<string, IPropertyAccessorInfo>(8);
            var instanceProperties = targetType.GetProperties(BindingFlags.Instance | BindingFlags.Public);
            for (var i = 0; i < instanceProperties.Length; i++)
            {
                var propertyName = instanceProperties[i].Name;
                var accessor = new ReflectionPropertyAccessor(this.m_TargetType, propertyName);
                this.PropertyAccessores.Add(propertyName.ToLower(), accessor);
            }
        }

        /// <summary>
        /// Gets the value stored in the property for 
        /// the specified target.
        /// </summary>
        /// <param name="target">Object to retrieve the property from.</param>
        /// <param name="propertyName"></param>
        /// <returns>Property value.</returns>
        public object GetValue(object target, string propertyName)
        {
            ArgumentAssertion.IsNotNull(target, "target");

            try
            {
                var accessor = this.GetPropertyAccessor(propertyName);
                return accessor.Get(target);
            }
            catch (PropertyAccessorException)
            {
                return null;
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null && ex.InnerException is NotSupportedException)
                {
                    LogManager.GetLogger().Warn("NotSupported Access {0}.get_{1} : {2}", target.GetType().Name, propertyName, ex.InnerException.Message);
                    return null;
                }
                throw new PropertyAccessorException(string.Format("Access {0}.get_{1} error : {2}", target.GetType().Name, propertyName, ex.Message), ex);
            }
        }

        /// <summary>
        /// Sets the value for the property of
        /// the specified target.
        /// </summary>
        /// <param name="target">Object to set the property on.</param>
        /// <param name="propertyName"></param>
        /// <param name="value">Property value.</param>
        public void SetValue(object target, string propertyName, object value)
        {
            ArgumentAssertion.IsNotNull(target, "target");

            try
            {
                var accessor = this.GetPropertyAccessor(propertyName);
                accessor.Set(target, value);
            }
            catch (PropertyAccessorException)
            {
                return;
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null && ex.InnerException is NotSupportedException)
                {
                    LogManager.GetLogger().Warn("NotSupported Access {0}.set_{1} : {2}", target.GetType().Name, propertyName, ex.InnerException.Message);
                    return;
                }
                throw new PropertyAccessorException(string.Format("Access {0}.set_{1} error : {2}", target.GetType().Name, propertyName, ex.Message), ex);
            }
        }

        private IPropertyAccessor GetPropertyAccessor(string propertyName)
        {            
            IPropertyAccessor accessor = null;

            var propertyKey = propertyName.ToLower();
            if (this.PropertyAccessores.ContainsKey(propertyKey))
            {
                accessor = this.PropertyAccessores[propertyKey];
            }
            return accessor;
        }
    }
}
