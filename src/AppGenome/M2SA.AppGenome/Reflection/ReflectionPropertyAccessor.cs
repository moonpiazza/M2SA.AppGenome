using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;
using System.Text;

namespace M2SA.AppGenome.Reflection
{
    /// <summary>
    /// The EmitPropertyAccessor class provides fast dynamic access
    /// to a property of a specified target class.
    /// </summary>
    public class ReflectionPropertyAccessor : IPropertyAccessorInfo
    {
        private Type mTargetType;
        private string mProperty;
        private Type mPropertyType;
        private PropertyInfo PropertyInfo;
        private FieldInfo FieldInfo;
        private bool mCanRead;
        private bool mCanWrite;

        /// <summary>
        /// 
        /// </summary>
        public string Name
        {
            get
            {
                return mProperty;
            }
        }

        /// <summary>
        /// Whether or not the Property supports read access.
        /// </summary>
        public bool CanRead
        {
            get
            {
                return this.mCanRead;
            }
        }

        /// <summary>
        /// Whether or not the Property supports write access.
        /// </summary>
        public bool CanWrite
        {
            get
            {
                return this.mCanWrite;
            }
        }

        /// <summary>
        /// The MetaType of object this property accessor was
        /// created for.
        /// </summary>
        public Type TargetType
        {
            get
            {
                return this.mTargetType;
            }
        }

        /// <summary>
        /// The MetaType of the Property being accessed.
        /// </summary>
        public Type PropertyType
        {
            get
            {
                return this.mPropertyType;
            }
        }

        /// <summary>
        /// Creates a new property accessor.
        /// </summary>
        /// <param name="targetType">Target object type.</param>
        /// <param name="property">Property name.</param>
        public ReflectionPropertyAccessor(Type targetType, string property)
        {
            ArgumentAssertion.IsNotNull(targetType, "targetType");

            this.mTargetType = targetType;

            this.PropertyInfo = targetType.GetProperty(property, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public);
            this.FieldInfo = targetType.GetField(property, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public);
            //
            // Make sure the property exists
            //
            if (this.PropertyInfo == null && this.FieldInfo == null)
            {
                throw new PropertyAccessorException(
                    string.Format("Property \"{0}\" does not exist for type "
                                  + "{1}.", property, targetType));
            }
            else if (this.PropertyInfo != null)
            {
                this.mCanRead = this.PropertyInfo.CanRead;
                this.mCanWrite = this.PropertyInfo.CanWrite;
                this.mProperty = this.PropertyInfo.Name;
                this.mPropertyType = this.PropertyInfo.PropertyType;
            }
            else
            {
                this.mCanRead = true;
                this.mCanWrite = true;
                this.mProperty = this.FieldInfo.Name;
                this.mPropertyType = this.FieldInfo.FieldType;
            }
        }

        /// <summary>
        /// Gets the property value from the specified target.
        /// </summary>
        /// <param name="target">Target object.</param>
        /// <returns>Property value.</returns>
        public object Get(object target)
        {
            if (mCanRead)
            {
                if (this.PropertyInfo != null)
                {
                    return this.PropertyInfo.GetValue(target, null);
                }
                else
                {
                    return this.FieldInfo.GetValue(target);
                }
            }
            else
            {
                throw new PropertyAccessorException(
                    string.Format("Property \"{0}\" does not have a get method.",
                                  mProperty));
            }
        }

        /// <summary>
        /// Sets the property for the specified target.
        /// </summary>
        /// <param name="target">Target object.</param>
        /// <param name="value">Value to set.</param>
        public void Set(object target, object value)
        {
            if (mCanWrite)
            {
                var val = value.Convert(this.PropertyType);
                if (this.PropertyInfo != null)
                {                    
                    this.PropertyInfo.SetValue(target, val, null);
                }
                else
                {
                    this.FieldInfo.SetValue(target, val);
                }
            }
            else
            {
                throw new PropertyAccessorException(
                    string.Format("Property \"{0}\" does not have a set method.",
                                  mProperty));
            }
        }


    }
}
