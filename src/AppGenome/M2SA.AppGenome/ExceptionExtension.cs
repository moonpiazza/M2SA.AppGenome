using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

using M2SA.AppGenome.Reflection;
using M2SA.AppGenome.ExceptionHandling;

namespace M2SA.AppGenome
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class FatalException : Exception
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected FatalException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public FatalException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="innerException"></param>
        public FatalException(Exception innerException)
            : base(null == innerException ? "" : innerException.Message, innerException)
        {            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public FatalException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public FatalException()
            : base()
        {
        }
    }

    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class TaskThreadException : Exception
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected TaskThreadException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public TaskThreadException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="innerException"></param>
        public TaskThreadException(Exception innerException)
            : base(null == innerException ? "" : innerException.Message, innerException)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public TaskThreadException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// 
        /// </summary>        
        public TaskThreadException()
            : base()
        {
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public static class ExceptionExtension
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public static bool Handle(this Action action)
        {
            return action.Handle(null, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="action"></param>
        /// <param name="bizInfo"></param>
        /// <returns></returns>
        public static bool Handle(this Action action, IDictionary bizInfo)
        {
            return action.Handle(null, bizInfo);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="action"></param>
        /// <param name="policyName"></param>
        /// <returns></returns>
        public static bool Handle(this Action action, string policyName)
        {
            return action.Handle(policyName, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="action"></param>
        /// <param name="policyName"></param>
        /// <param name="bizInfo"></param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        public static bool Handle(this Action action, string policyName, IDictionary bizInfo)
        {
            ArgumentAssertion.IsNotNull(action, "action");

            try
            {
                action();
                return false;
            }
            catch (Exception ex)
            {
                return ex.HandleException(policyName, bizInfo);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool HandleException(this Exception source)
        {
            return source.HandleException(null, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="bizInfo"></param>
        /// <returns></returns>
        public static bool HandleException(this Exception source, IDictionary bizInfo)
        {
            return source.HandleException(null, bizInfo);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="policyName"></param>
        /// <returns></returns>
        /// <example>
        /// The following code shows the usage of the 
        /// exception handling framework.
        /// <code>
        /// try
        ///	{
        ///		DoWork();
        ///	}
        ///	catch (Exception e)
        ///	{
        ///		if (e.HandleException(policyName)) throw;
        ///	}
        /// </code>
        /// </example>
        public static bool HandleException(this Exception source, string policyName)
        {
            return source.HandleException(policyName, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="policyName"></param>
        /// <param name="bizInfo"></param>
        /// <returns></returns>
        public static bool HandleException(this Exception source, string policyName, IDictionary bizInfo)
        {
            var policy = GetExceptionPolicy(policyName);
            return policy.HandleException(source, bizInfo);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="policyName"></param>
        /// <param name="exceptionToThrow"></param>
        /// <returns></returns>
        /// <example>
        /// The following code shows the usage of the 
        /// exception handling framework.
        /// <code>
        /// try
        ///	{
        ///		DoWork();
        ///	}
        ///	catch (Exception e)
        ///	{
        ///	    Exception exceptionToThrow;
        ///		if (ExceptionPolicy.HandleException(e, name, out exceptionToThrow))
        ///		{
        ///		  if(exceptionToThrow == null)
        ///		    throw;
        ///		  else
        ///		    throw exceptionToThrow;
        ///		}
        ///	}
        /// </code>
        /// </example>
        public static bool HandleException(this Exception source, string policyName, out Exception exceptionToThrow)
        {
            return source.HandleException(policyName, null, out exceptionToThrow);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="policyName"></param>
        /// <param name="exceptionToThrow"></param>
        /// <param name="bizInfo"></param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        public static bool HandleException(this Exception source, string policyName, IDictionary bizInfo, out Exception exceptionToThrow)
        {
            ArgumentAssertion.IsNotNull(source, "source");

            try
            {
                bool retrowAdviced = source.HandleException(policyName, bizInfo);
                exceptionToThrow = null;

                return retrowAdviced;
            }
            catch (Exception exception)
            {
                exceptionToThrow = exception;
                return true;
            }
        }

        static IExceptionPolicy GetExceptionPolicy(string policyName)
        {            
            return ObjectIOCFactory.GetSingleton<IExceptionPolicyFactory>().GetPolicy(policyName);
        }
    }
}
