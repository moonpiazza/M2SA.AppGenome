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
    public static class DbParamsExtension 
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

            var targetProperties = entity.GetType().GetPersistProperties();
            var propNames = new List<string>(targetProperties.Count);
            foreach (var pair in targetProperties)
            {
                if(pair.Value.IsPrimitiveType())
                    propNames.Add(pair.Key);
            }

            var propertyValues = entity.GetPropertyValues(propNames);
            return propertyValues;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dataTable"></param>
        /// <returns></returns>
        public static IList<T> Convert<T>(this DataTable dataTable) where T : new()
        {
            ArgumentAssertion.IsNotNull(dataTable, "dataTable");

            IList<T> list = null;
            var itemCount = dataTable.Rows.Count;
            if (itemCount > 0)
            {
                list = new List<T>(itemCount);
                for (var i = 0; i < itemCount; i++)
                {
                    var item = dataTable.Rows[i].Serialize<T>();
                    list.Add(item);
                }
            }
            return list;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="row"></param>
        /// <returns></returns>
        public static T Serialize<T>(this DataRow row) where T : new()
        {
            var entity = new T();
            row.Serialize<T>(entity);
            return entity;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="row"></param>
        /// <param name="entity"></param>
        public static T Serialize<T>(this DataRow row, T entity)
        {
            ArgumentAssertion.IsNotNull(row, "row");
            ArgumentAssertion.IsNotNull(entity, "entity");

            if (entity is IEntity)
                (entity as IEntity).PersistentState = PersistentState.Persistent;

            var targetType = entity.GetType();
            var targetProperties = targetType.GetPersistProperties();

            var propertyValues = new Dictionary<string, object>(targetProperties.Count);

            foreach (DataColumn column in row.Table.Columns)
            {
                foreach (var prop in targetProperties)
                {
                    if (prop.Value.IsPrimitiveType() && prop.Key.ToLower() == column.ColumnName.ToLower())
                    {
                        if (row[column.ColumnName] != null && row[column.ColumnName] != DBNull.Value)
                        {
                            propertyValues.Add(prop.Key, row[column.ColumnName]);
                        }                        
                        break;
                    }             
                }
            }

            entity.SetPropertyValues(propertyValues);
            
            return entity;
        }

    }
}
