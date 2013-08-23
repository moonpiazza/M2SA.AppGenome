using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;
using System.Messaging;
using M2SA.AppGenome.Configuration;

namespace M2SA.AppGenome.Queues
{
    /// <summary>
    /// 
    /// </summary>
    public class QueueFactory : ResolveFactoryBase<IMessageQueue>, IQueueFactory
    {
        /// <summary>
        /// 
        /// </summary>
        public readonly static string QueueLogger = "Queue";

        /// <summary>
        /// 
        /// </summary>
        protected override string ModuleKey
        {
            get
            {
                return AppConfig.QueuesKey;
            }
        }

        private QueueFactory()
        {
            AppInstance.RegisterTypeAliasByModule<QueueCluster>(AppConfig.QueuesKey);
            AppInstance.RegisterTypeAliasByModule<MSMQ>(AppConfig.QueuesKey);
            AppInstance.RegisterTypeAliasByModule<LoadStrategies.RandomLoadStrategy>(AppConfig.QueuesKey);

        }

        /// <summary>
        /// 加载指定的配置信息
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        protected override IConfigNode LoadConfigInfo(string name)
        {
            var nodePath = string.Format("/queue[@name='{0}']", name);
            var configNode = AppInstance.GetConfigNode(this.ModuleKey, nodePath);
            return configNode;
        }

        #region IQueueFactory 成员

        /// <summary>
        /// 
        /// </summary>
        /// <param name="queueName"></param>
        /// <returns></returns>
        public IMessageQueue GetQueue(string queueName)
        {
            return this.GetInstance(queueName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataType"></param>
        /// <returns></returns>
        public IMessageQueue GetQueueByDataType(string dataType)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="queueName"></param>
        /// <returns></returns>
        public bool ExistsQueue(string queueName)
        {
            return this.Exists(queueName);
        }

        #endregion
    }
}
