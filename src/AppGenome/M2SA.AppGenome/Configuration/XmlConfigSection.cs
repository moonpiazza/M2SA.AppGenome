using System;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace M2SA.AppGenome.Configuration
{
    /// <summary>
    /// 
    /// </summary>
    public class XmlConfigSection : ConfigurationSection
    {
        private string _rawXml;

        /// <summary>
        /// 
        /// </summary>
        public string RawXml
        {
            get
            {
                return this._rawXml;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        protected override void DeserializeSection(XmlReader reader)
        {
            if (null == reader || !reader.Read() || (reader.NodeType != XmlNodeType.Element))
            {
                throw new ConfigurationErrorsException("Config_base_expected_to_find_element");
            }
            this._rawXml = reader.ReadOuterXml();

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parentElement"></param>
        protected override void Reset(ConfigurationElement parentElement)
        {
            this._rawXml = string.Empty;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parentElement"></param>
        /// <param name="name"></param>
        /// <param name="saveMode"></param>
        /// <returns></returns>
        protected override string SerializeSection(ConfigurationElement parentElement, string name, ConfigurationSaveMode saveMode)
        {
            return this._rawXml;
        }


    }
}
