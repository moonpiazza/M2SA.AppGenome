using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Data;
using M2SA.AppGenome.Data.SqlMap;
using M2SA.AppGenome.Reflection;

namespace M2SA.AppGenome.Data
{
    /// <summary>
    /// 
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1012:AbstractTypesShouldNotHaveConstructors")]
    public abstract class SimpleRepositoryBase<T, TId> : IRepository<T, TId>
        where T : class, IEntity<TId>, new()
    {
        string moduleName = null;

        /// <summary>
        /// 
        /// </summary>
        public SimpleRepositoryBase()
        {
            this.moduleName = this.GetType().FullName;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sqlName"></param>
        /// <returns></returns>
        protected string FormatSqlName(string sqlName)
        {
            return string.Concat(this.moduleName, SqlMapping.SqKeySeparator[0], sqlName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataRow"></param>
        /// <returns></returns>
        protected virtual T Convert(DataRow dataRow)
        {
            return dataRow.Serialize<T>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataTable"></param>
        /// <returns></returns>
        protected virtual IList<T> Convert(DataTable dataTable)
        {
            ArgumentAssertion.IsNotNull(dataTable, "dataTable");

            IList<T> list = null;
            var itemCount = dataTable.Rows.Count;
            if (itemCount > 0)
            {
                list = new List<T>(itemCount);
                for (var i = 0; i < itemCount; i++)
                {
                    var item = this.Convert(dataTable.Rows[i]);
                    list.Add(item);
                }
            }
            return list;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        protected virtual IDictionary<string, object> Convert(T entity)
        {
            var parameterValues = entity.ToDictionary();
            return parameterValues;
        }

        #region IRepository<T> 成员

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual T CreateNew()
        {
            var entity = new T();
            entity.PersistentState = PersistentState.Transient;
            return entity;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual T FindById(TId id)
        {
            var sqlName = this.FormatSqlName("SelectById");
            var pValues = new Dictionary<string, object>(1);
            pValues.Add("Id", id);

            var dataset = SqlHelper.ExecuteDataSet(sqlName, pValues);

            T result = default(T);
            if (dataset.Tables[0].Rows.Count == 1)
            {
                result = this.Convert(dataset.Tables[0].Rows[0]);
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual IList<T> LoadAll()
        {
            var sqlName = this.FormatSqlName("SelectAll");
            var dataset = SqlHelper.ExecuteDataSet(sqlName, null);

            IList<T> list = null;
            var itemCount = dataset.Tables[0].Rows.Count;
            if (itemCount > 0)
            {
                list = new List<T>(itemCount);
                for (var i = 0; i < itemCount; i++)
                {
                    var item = this.Convert(dataset.Tables[0].Rows[i]);
                    list.Add(item);
                }
            }
            return list;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public virtual bool Save(T model)
        {
            var result = false;

            if (model.PersistentState == PersistentState.Transient)
            {
                var sqlName = this.FormatSqlName("Insert");
                var pValues = this.Convert(model);
                var identity = SqlHelper.ExecuteIdentity<TId>(sqlName, pValues);

                var defaultValue = default(TId);
                if (defaultValue != null && false == defaultValue.Equals(identity))
                {
                    model.Id = identity;
                }
                result = defaultValue == null ? model.Id != null : false == defaultValue.Equals(model.Id);
            }
            else
            {
                var sqlName = this.FormatSqlName("Update");
                var pValues = this.Convert(model);
                pValues["Id"] = model.Id;
                var rowsAffected = SqlHelper.ExecuteNonQuery(sqlName, pValues);
                result = rowsAffected > 0;
            }
            if (result == true)
            {
                model.PersistentState = PersistentState.Persistent;
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public virtual bool Remove(T model)
        {
            var sqlName = this.FormatSqlName("DeleteById");
            var pValues = new Dictionary<string, object>(1);
            pValues.Add("Id", model.Id);

            var rowsAffected = SqlHelper.ExecuteNonQuery(sqlName, pValues);
            model.PersistentState = PersistentState.Deleted;

            var result = rowsAffected > 0;
            return result;
        }

        #endregion
    }
}
