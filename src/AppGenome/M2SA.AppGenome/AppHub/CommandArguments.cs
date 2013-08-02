using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace M2SA.AppGenome.AppHub
{
    /// <summary>
    /// 
    /// </summary>
    public class CommandArguments
    {
        private IDictionary<string, string> argumentMap = null;

        /// <summary>
        /// 
        /// </summary>
        public string SessionId
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public int HostProcessId
        {
            get;
            private set;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        public CommandArguments(string[] args)
        {
            ArgumentAssertion.IsNotNull(args, "args");

            this.argumentMap = new Dictionary<string, string>(args.Length);
            foreach (var arg in args)
            {
                var argInfo = arg.Split(':');
                var argName = argInfo[0].ToLower();
                if (argName.StartsWith("-"))
                    argName = argName.Substring(1);
                string argValue = string.Empty;
                if (argInfo.Length > 1)
                    argValue = argInfo[1];
                this.argumentMap.Add(argName, argValue);
            }

            if (this.ContainsArgument("appId"))
            {
                this.SessionId = this.GetCommandArgument("appId");

                if (this.ContainsArgument("hostPID"))
                {
                    var hostPIDStr = this.GetCommandArgument("hostPID");
                    this.HostProcessId = int.Parse(hostPIDStr);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="argName"></param>
        /// <returns></returns>
        public string GetCommandArgument(string argName)
        {
            ArgumentAssertion.IsNotNull(argName, "argName");

            string value = null;
            argName = argName.ToLower();
            if (this.argumentMap.ContainsKey(argName))
            {
                value = this.argumentMap[argName];
            }

            return value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="argName"></param>
        /// <returns></returns>
        public bool ContainsArgument(string argName)
        {
            ArgumentAssertion.IsNotNull(argName, "argName");

            argName = argName.ToLower();
            return this.argumentMap.ContainsKey(argName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var result = new StringBuilder(128);
            foreach (var item in this.argumentMap)
            {
                if (string.IsNullOrEmpty(item.Value))
                {
                    result.AppendFormat("-{0} ", item.Key);
                }
                else
                {
                    result.AppendFormat("-{0}:{1} ", item.Key, item.Value);
                }
            }

            return result.ToString();
        }
    }
}
