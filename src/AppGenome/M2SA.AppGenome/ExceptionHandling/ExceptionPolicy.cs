using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using M2SA.AppGenome.Configuration;
using M2SA.AppGenome.Logging;
using M2SA.AppGenome.Reflection;
using M2SA.AppGenome.Web;

namespace M2SA.AppGenome.ExceptionHandling
{
    /// <summary>
    /// 
    /// </summary>
    public class ExceptionPolicy : ResolveObjectBase, IExceptionPolicy
    {
        internal static ExceptionPolicy CreateDefaultPolicy()
        {
            var entryForException = new ExceptionPolicyEntry();
            entryForException.Name = "SystemException";
            entryForException.ExceptionType = "Exception";
            entryForException.PostHandlingAction = PostHandlingAction.NotifyRethrow;
            entryForException.Handlers = new List<IExceptionHandler>(1){ 
                new LoggingExceptionHandler()
                {
                    LogCategory = "ExceptionLogger",
                    LogLevel = LogLevel.Error
                }
            };

            var entryForFatalException = new ExceptionPolicyEntry();
            entryForFatalException.Name = "FatalException";
            entryForFatalException.ExceptionType = "FatalException";
            entryForFatalException.PostHandlingAction = PostHandlingAction.NotifyRethrow;
            entryForFatalException.Handlers = new List<IExceptionHandler>(1){ 
                new LoggingExceptionHandler()
                {
                    LogCategory = "FatalExceptionLogger",
                    LogLevel = LogLevel.Fatal
                }
            };

            var entryForTaskException = new ExceptionPolicyEntry();
            entryForTaskException.Name = "TaskThreadException";
            entryForTaskException.ExceptionType = "TaskThreadException";
            entryForTaskException.PostHandlingAction = PostHandlingAction.NotifyRethrow;
            entryForTaskException.Handlers = new List<IExceptionHandler>(1){ 
                new LoggingExceptionHandler()
                {
                    LogCategory = "TaskThreadExceptionLogger",
                    LogLevel = LogLevel.Error
                }
            };

            var policy = new ExceptionPolicy();
            policy.PolicyEntries = new List<ExceptionPolicyEntry>(3) { entryForException, entryForFatalException, entryForTaskException };
            policy.InitPolicyMap();

            return policy;
        }

        IDictionary<Type, ExceptionPolicyEntry> policyMap = null;

        /// <summary>
        /// 
        /// </summary>
        public List<ExceptionPolicyEntry> PolicyEntries
        {
            get;
            private set;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="config"></param>
        public override void Initialize(IConfigNode config)
        {
            base.Initialize(config);
            this.InitPolicyMap();
        }

        private void InitPolicyMap()
        {
            if (this.PolicyEntries == null)
            {
                this.policyMap = new Dictionary<Type, ExceptionPolicyEntry>(0);
            }
            else
            {
                this.policyMap = new Dictionary<Type, ExceptionPolicyEntry>(this.PolicyEntries.Count);
                foreach (var item in this.PolicyEntries)
                {
                    var type = TypeExtension.GetMapType(item.ExceptionType);
                    if (null == type)
                    {
                        throw new ConfigException(string.Format("cannot find the ExceptionType :{0}", item.ExceptionType));
                    }

                    this.policyMap[type] = item;
                }
            }
        }

        #region IExceptionPolicy 成员

        /// <summary>
        /// 
        /// </summary>
        /// <param name="originalException"></param>
        /// <param name="bizInfo"></param>
        /// <returns></returns>
        public bool HandleException(Exception originalException, IDictionary bizInfo)
        {
            ArgumentAssertion.IsNotNull(originalException, "originalException");

            ExceptionPolicyEntry entry = GetPolicyEntry(originalException);

            if (entry == null)
            {
                return true;
            }

            originalException = FindOriginalException(originalException);

            if ((originalException is HostileRequestException) == false && HttpContext.Current != null && HttpValidator.IsCrawlerRequest(HttpContext.Current.Request))
            {
                var msg = string.Format("[CrawlerRequest]{0}", originalException.Message);
                originalException = new HostileRequestException(msg, originalException);
                entry = GetPolicyEntry(originalException);
            }            

            return entry.Handle(originalException, bizInfo);
        }

        #endregion

        static Exception FindOriginalException(Exception ex)
        {
            var originalException = ex;
            while (originalException is HttpUnhandledException && originalException.InnerException != null)
            {
                originalException = originalException.InnerException;
            }

            return originalException;
        }

        ExceptionPolicyEntry GetPolicyEntry(Exception ex)
        {
            Type exceptionType = ex.GetType();
            ExceptionPolicyEntry entry = this.FindExceptionPolicyEntry(exceptionType);
            return entry;
        }        

        ExceptionPolicyEntry FindExceptionPolicyEntry(Type exceptionType)
        {
            ExceptionPolicyEntry entry = null;

            while (exceptionType != typeof(Object))
            {
                entry = GetPolicyEntry(exceptionType);

                if (entry != null)
                {
                    break;
                }
                else
                {
                    exceptionType = exceptionType.BaseType;
                }
            }

            return entry;
        }

        ExceptionPolicyEntry GetPolicyEntry(Type exceptionType)
        {
            if (this.policyMap.ContainsKey(exceptionType))
            {
                return this.policyMap[exceptionType];
            }
            return null;
        }
    }
}
