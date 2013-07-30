using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using M2SA.AppGenome.Configuration;

namespace M2SA.AppGenome.ExceptionHandling
{
    /// <summary>
    /// 
    /// </summary>
    public class ExceptionPolicyFactory : ResolveFactoryBase<IExceptionPolicy>, IExceptionPolicyFactory
    {
        /// <summary>
        /// 
        /// </summary>
        protected override string ModuleKey
        {
            get
            {
                return AppConfig.ExceptionHandlingKey;
            }
        }

        /// <summary>
        /// 加载指定的配置信息
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        protected override IConfigNode LoadConfigInfo(string name)
        {
            var nodePath = string.Format("exceptionPolicy[@name='{0}']", name);
            var configNode = AppInstance.GetConfigNode(this.ModuleKey, nodePath);
            return configNode;
        }

        #region IExceptionPolicyFactory 成员

        /// <summary>
        /// 
        /// </summary>
        /// <param name="policyName"></param>
        /// <returns></returns>
        public IExceptionPolicy GetPolicy(string policyName)
        {
            return this.GetInstance(policyName);
        }

        #endregion
    }
}
