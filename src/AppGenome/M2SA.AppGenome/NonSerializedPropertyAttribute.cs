using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace M2SA.AppGenome
{
    /// <summary>
    /// 指示可序列化类的某个字段不应被序列化。无法继承此类。
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = false)]
    public sealed class NonSerializedPropertyAttribute : Attribute
    {
        /// <summary>
        /// 
        /// </summary>
        public NonSerializedPropertyAttribute()
        {
        }
    }
}
