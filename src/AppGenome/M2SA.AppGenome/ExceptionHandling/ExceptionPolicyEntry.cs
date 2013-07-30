using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace M2SA.AppGenome.ExceptionHandling
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class ExceptionPolicyEntry
    {
        /// <summary>
        /// 
        /// </summary>
        public string Name
        {
            get;
            private set;
        }

        /// <summary>
        /// 
        /// </summary>
        public string ExceptionType
        {
            get;
            private set;
        }

        /// <summary>
        /// 
        /// </summary>
        public PostHandlingAction PostHandlingAction
        {
            get;
            private set;
        }

        /// <summary>
        /// 
        /// </summary>
        public IList<IExceptionHandler> Handlers
        {
            get;
            private set;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="originalException"></param>
        /// <param name="bizInfo"></param>
        /// <returns></returns>
        public bool Handle(Exception originalException, IDictionary bizInfo)
        {
            if (originalException == null)
                throw new ArgumentNullException("originalException");

            Guid handlingInstanceID = Guid.NewGuid();
            Exception chainException = ExecuteHandlerChain(originalException, handlingInstanceID, bizInfo);

            return RethrowRecommended(chainException, originalException);
        }

        static Exception IntentionalRethrow(Exception chainException, Exception originalException)
        {
            if (chainException != null)
            {
                throw chainException;
            }

            Exception wrappedException = new ExceptionHandlingException(originalException.Message, originalException); 
            return wrappedException;
        }

        bool RethrowRecommended(Exception chainException, Exception originalException)
        {
            if (this.PostHandlingAction == PostHandlingAction.None)
                return false;
            if (this.PostHandlingAction == PostHandlingAction.ThrowNewException)
                throw IntentionalRethrow(chainException, originalException);

            return true;
        }

        Exception ExecuteHandlerChain(Exception originalException, Guid handlingInstanceID, IDictionary bizInfo)
        {
            string lastHandlerName = String.Empty;
            try
            {
                foreach (IExceptionHandler handler in this.Handlers)
                {
                    lastHandlerName = handler.GetType().Name;
                    originalException = handler.HandleException(originalException, handlingInstanceID, bizInfo);
                }
            }
            catch (Exception handlingException)
            {
                throw new ExceptionHandlingException(lastHandlerName, handlingException);
            }

            return originalException;
        }
    }
}
