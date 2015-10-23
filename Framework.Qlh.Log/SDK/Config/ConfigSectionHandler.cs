using System;
using System.Xml;
using System.Configuration;

namespace Framework.Qlh.Log.Config
{
    public class ConfigSectionHandler : IConfigurationSectionHandler
    {
        public object Create(object parent, object configContext, XmlNode section)
        {
            string configFileName = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;

            return new XmlLoggingConfiguration(configFileName);
        }
    }
}
