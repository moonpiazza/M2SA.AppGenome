using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Reflection;
using M2SA.AppGenome.Reflection;

namespace M2SA.AppGenome.Data
{
    /// <summary>
    /// 
    /// </summary>
    public static class DBParamsExtension 
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static IDictionary<string, object> ToDictionary(this object entity)
        {
            if (entity == null)
                return null;

            var propertyValues =  entity.GetPropertyValues();
            return propertyValues;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="row"></param>
        /// <param name="entity"></param>
        public static void Serialize<T>(this DataRow row, T entity)
        {
            var targetType = entity.GetType();
            var targetProperties = targetType.GetPersistProperties();

            var propertyValues = new Dictionary<string, object>(targetProperties.Count);

            foreach (DataColumn column in row.Table.Columns)
            {
                foreach (var prop in targetProperties)
                {
                    if (prop.Key.ToLower() == column.ColumnName.ToLower())
                    {
                        if (row[column.ColumnName] != DBNull.Value)
                        {
                            propertyValues.Add(prop.Key, row[column.ColumnName]);
                        }                        
                        break;
                    }             
                }
            }

            entity.SetPropertyValues(propertyValues);
        }
    }
}
