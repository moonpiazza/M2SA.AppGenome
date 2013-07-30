using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace M2SA.AppGenome.Threading
{
    /// <summary>
    /// 后台任务接口
    /// </summary>
    public interface ITaskAction
    {
        /// <summary>
        /// 是否可以被取消
        /// </summary>
        bool CanCancel
        {
            get;
        }

        /// <summary>
        /// 执行方法
        /// </summary>
        void Invoke();

        /// <summary>
        /// 是否触发
        /// </summary>
        /// <param name="waitInterval"></param>
        /// <returns></returns>
        bool Trigger(TimeSpan waitInterval);
    }
}
