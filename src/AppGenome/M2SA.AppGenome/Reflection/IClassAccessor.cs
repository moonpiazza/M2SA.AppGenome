using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace M2SA.AppGenome.Reflection
{
    /// <summary>
    /// 
    /// </summary>
    public interface IClassAccessor
    {
        /// <summary>
        /// 类型的实例属性
        /// </summary>
        IDictionary<string, IPropertyAccessorInfo> PropertyAccessores
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the value stored in the property for 
        /// the specified target.
        /// </summary>
        /// <param name="target">Object to retrieve the property from.</param>
        /// <param name="propertyName"></param>
        /// <returns>Property value.</returns>
        object GetValue(object target, string propertyName);

        /// <summary>
        /// Sets the value for the property of
        /// the specified target.
        /// </summary>
        /// <param name="target">Object to set the property on.</param>
        /// <param name="propertyName"></param>
        /// <param name="value">Property value.</param>
        void SetValue(object target, string propertyName, object value);
    }
}
