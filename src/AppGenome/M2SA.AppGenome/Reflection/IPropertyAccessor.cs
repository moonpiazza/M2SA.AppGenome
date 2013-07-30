using System;
using System.Collections.Generic;
using System.Text;

namespace M2SA.AppGenome.Reflection
{
    /// <summary>
    /// The IPropertyAccessor interface defines a property
    /// accessor.
    /// </summary>
    public interface IPropertyAccessor
    {
        /// <summary>
        /// Gets the value stored in the property for 
        /// the specified target.
        /// </summary>
        /// <param name="target">Object to retrieve
        /// the property from.</param>
        /// <returns>Property value.</returns>
        object Get(object target);

        /// <summary>
        /// Sets the value for the property of
        /// the specified target.
        /// </summary>
        /// <param name="target">Object to set the
        /// property on.</param>
        /// <param name="value">Property value.</param>
        void Set(object target, object value);
    }

    /// <summary>
    /// 
    /// </summary>
    public interface IPropertyAccessorInfo : IPropertyAccessor
    {
        /// <summary>
        /// 
        /// </summary>
        string Name
        {
            get;
        }

        /// <summary>
        /// 
        /// </summary>
        bool CanRead
        {
            get;
        }

        /// <summary>
        /// 
        /// </summary>
        bool CanWrite
        {
            get;
        }

        /// <summary>
        /// 
        /// </summary>
        Type PropertyType
        {
            get;
        }
    }

    internal class PropertyAccessorUtil
    {
        internal static object GetBoolByValue(Type t, object value)
        {
            if (value == null)
                return value;

            if (t == typeof(bool) ||
                t == typeof(Boolean))
            {
                bool valueBool = false;
                bool.TryParse(value.ToString(), out valueBool);
                if (!valueBool)
                {
                    if (value.ToString() == "1")
                        valueBool = true;
                }
                return valueBool;
            }

            return value;
        }
    }
}
