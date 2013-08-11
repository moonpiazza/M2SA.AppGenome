using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using M2SA.AppGenome.Configuration;

namespace M2SA.AppGenome.Cache
{
    /// <summary>
    /// 缓存接口
    /// </summary>
    public interface ICache : IResolveObject
    {
        /// <summary>
        /// 缓存项操作接口
        /// </summary>
        ILoadDataHandler LoadDataHandler
        {
            get;
        }

        /// <summary>
        /// 缓存名称
        /// </summary>
        string Name
        {
            get;
        }

        /// <summary>
        /// 缓存有效期
        /// </summary>
        TimeSpan ExpiryTime
        {
            get;
            set;
        }

         /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        object Get(string key);

        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        T Get<T>(object key);

        /// <summary>
        /// 保存数据
        /// </summary>
        /// <param name="key"></param>
        /// <param name="data"></param>
        void Set(string key, object data);

        /// <summary>
        /// 保存数据
        /// </summary>
        /// <param name="key"></param>
        /// <param name="data"></param>
        void Set(object key, object data);

        /// <summary>
        /// 移除数据
        /// </summary>
        /// <param name="key"></param>
        void Remove(string key);

        /// <summary>
        /// 获取缓存状态信息
        /// </summary>
        /// <returns></returns>
        object GetState();
    }
}
