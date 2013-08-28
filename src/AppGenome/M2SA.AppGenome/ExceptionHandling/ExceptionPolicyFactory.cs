using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using M2SA.AppGenome.Configuration;
using M2SA.AppGenome.Logging;

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

        IDictionary<string, IConfigNode> ConfigMap = null;

        private ExceptionPolicyFactory()
        {
            this.ConfigMap = new Dictionary<string, IConfigNode>();
        }

        /// <summary>
        /// 加载指定的配置信息
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        protected override IConfigNode LoadConfigInfo(string name)
        {
            if (this.ConfigMap.ContainsKey(name) == false)
            {
                var nodePath = string.Format("/exceptionPolicy[@name='{0}']", name);
                var configNode = AppInstance.GetConfigNode(this.ModuleKey, nodePath);
                if (null == configNode)
                {
                    var defaultCategory = this.GetDefaultCategory();
                    if (name == defaultCategory)
                        return null;
                    else
                        return this.LoadConfigInfo(defaultCategory);
                }

                this.ConfigMap[name] = configNode;
            }
            return this.ConfigMap[name];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        protected override IExceptionPolicy ResolveObject(string name)
        {
            if (this.ConfigMap.ContainsKey(name) == false)
            {
                var defaultCategory = this.GetDefaultCategory();
                if (name == defaultCategory)
                {
                    return ExceptionPolicy.CreateDefaultPolicy();
                }
                else 
                {
                    return this.GetPolicy(defaultCategory);
                }
            }
            else
            {
                var obj = base.ResolveObject(name);
                return obj;
            }
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
