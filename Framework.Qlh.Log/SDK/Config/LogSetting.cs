using System.Configuration;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using WindowsServer.Configuration;

namespace Framework.Qlh.Log
{
    public class LogSetting : IConfigurationSectionHandler
    {
        public string AppNo { get; set; }

        public AppTraceSetting AppTrace { get; set; }

        public AppLogSetting AppLog { get; set; }

        public SysLogSetting SysLog { get; set; }

        public virtual object Create(object parent, object configContext, XmlNode section)
        {
            var serialize = new XmlSerializer(typeof(LogSetting));
            var xDocument = XDocument.Parse(section.OuterXml, LoadOptions.SetBaseUri | LoadOptions.SetLineInfo);
            LogSetting t;
            using (var memoryStream = new MemoryStream())
            {
                xDocument.Save(memoryStream);
                memoryStream.Seek(0, SeekOrigin.Begin);
                t = (LogSetting)serialize.Deserialize(memoryStream);
            }

            if (string.IsNullOrEmpty(t.AppLog.ApiUrl))
            {
                t.AppLog.ApiUrl = ConfigurationCenter.Global["AppLogService"];
            }

            if (string.IsNullOrEmpty(t.SysLog.ApiUrl))
            {
                t.SysLog.ApiUrl = ConfigurationCenter.Global["SysLogService"];
            }

            if (string.IsNullOrEmpty(t.AppNo))
            {
                t.AppNo = ConfigurationCenter.Global["AppNo"];
            }

            return t;
        }
    }


    public class AppLogSetting : BaseLogSetting
    {
    }

    public class SysLogSetting : BaseLogSetting
    {
    }

    public class AppTraceSetting
    {
        public int Sequence { get; set; }

        public string ApiUrl { get; set; }


    }

    public abstract class BaseLogSetting
    {
        public string MinLevelNoCache { get; set; }
        public int MaxQueueNum { get; set; }
        public string ApiUrl { get; set; }

        /// <summary
        /// 自动写入log 的周期，单位是秒
        /// </summary
        public int AutoFlushPeriod { get; set; }
    }
}
