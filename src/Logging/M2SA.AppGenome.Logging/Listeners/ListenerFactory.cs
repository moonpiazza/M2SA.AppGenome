using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using M2SA.AppGenome.Configuration;

namespace M2SA.AppGenome.Logging.Listeners
{
    /// <summary>
    /// 
    /// </summary>
    public class ListenerFactory : ResolveFactoryBase<IListener>
    {
        /// <summary>
        /// 
        /// </summary>
        protected override string ModuleKey
        {
            get
            {
                return AppConfig.LoggingKey;
            }
        }

        /// <summary>
        /// 加载指定的配置信息
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        protected override IConfigNode LoadConfigInfo(string name)
        {
            var nodePath = string.Format("/listeners/listener[@name='{0}']", name);
            var configNode = AppInstance.GetConfigNode(this.ModuleKey, nodePath);
            if (null == configNode)
                throw new ArgumentOutOfRangeException("name", name, "cannot find the Listener");

            if (string.IsNullOrEmpty(configNode.MetaType))
                throw new ArgumentOutOfRangeException("name", name, "cannot find the Listener type");

            if (configNode.MetaType.StartsWith(AppConfig.LoggingKey) == false)
                configNode.MetaType = string.Format("{0}.{1}", AppConfig.LoggingKey, configNode.MetaType);

            return configNode;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public IListener GetListener(string name)
        {
            return this.GetInstance(name);
        }
    }
}
