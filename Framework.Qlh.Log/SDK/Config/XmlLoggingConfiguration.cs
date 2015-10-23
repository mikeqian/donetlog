using System;
using System.Collections;
using System.Configuration;
using System.Reflection;
using System.Xml;
using System.IO;

namespace Framework.Qlh.Log.Config
{
    public class XmlLoggingConfiguration : LoggingConfiguration
    {
        private Hashtable _visitedFile = new Hashtable();

        private bool _autoReload = false;
        private string _originalFileName = null;

        public bool AutoReload
        {
            get { return _autoReload; }
            set { _autoReload = value; }
        }

        public XmlLoggingConfiguration() { }
        public XmlLoggingConfiguration(string fileName)
        {
            _originalFileName = fileName;
            ConfigureFromFile(fileName);
        }

        public override ICollection FileNamesToWatch
        {
            get
            {
                if (_autoReload)
                    return _visitedFile.Keys;
                else
                    return null;
            }
        }

        public override LoggingConfiguration Reload()
        {
            return new XmlLoggingConfiguration(_originalFileName);
        }

        private void ConfigureFromFile(string fileName)
        {
            string key = Path.GetFullPath(fileName).ToLower();
            if (_visitedFile.Contains(key))
                return;

            _visitedFile[key] = this;

            var doc = new XmlDocument();
            doc.Load(fileName);
            if (doc.DocumentElement != null && doc.DocumentElement.LocalName == "configuration")
            {
                foreach (XmlElement el in doc.GetElementsByTagName("mLog"))
                {
                    ConfigureFromXmlElement(el, Path.GetDirectoryName(fileName));
                }
            }
            else
            {
                ConfigureFromXmlElement(doc.DocumentElement, Path.GetDirectoryName(fileName));
            }
        }

        private void ConfigureFromXmlElement(XmlElement configElement, string baseDirectory)
        {
            if (configElement.HasAttribute("autoReload"))
            {
                AutoReload = true;
            }

            foreach (XmlElement el in configElement.GetElementsByTagName("include"))
            {
                Layout layout = new Layout(el.GetAttribute("file"));

                string newFileName = layout.GetFormattedMessage(LogEventInfo.Empty);
                newFileName = Path.Combine(baseDirectory, newFileName);
                if (File.Exists(newFileName))
                {
                    ConfigureFromFile(newFileName);
                }
                else
                {
                    throw new FileNotFoundException("Included fine not found.", newFileName);
                }
            }

            foreach (XmlElement el in configElement.GetElementsByTagName("layout-appenders"))
            {
                AddLayoutAppendersFromElement(el);
            }

            foreach (XmlElement el in configElement.GetElementsByTagName("appenders"))
            {
                ConfigureAppendersFromElement(el);
            }

            foreach (XmlElement el in configElement.GetElementsByTagName("rules"))
            {
                ConfigureRulesFromElement(el);
            }

            ResolveAppenders();
        }

        public static LoggingConfiguration AppConfig
        {
            get
            {
                object o = ConfigurationManager.GetSection("mLog");
                return o as LoggingConfiguration;
            }
        }

        // implementation details

        private static string CleanWhitespace(string s)
        {
            s = s.Replace(" ", ""); // get rid of the whitespace
            return s;
        }

        private static LogLevel LogLevelFromString(string s)
        {
            switch (s)
            {
                case "Debug":
                    return LogLevel.Debug;
                case "Info":
                    return LogLevel.Info;
                case "Warn":
                    return LogLevel.Warn;
                case "Error":
                    return LogLevel.Error;
                case "Fatal":
                    return LogLevel.Fatal;
                default:
                    throw new ArgumentException("Unknown log level: " + s);
            }
        }

        private void ConfigureRulesFromElement(XmlElement element)
        {
            if (element == null)
                return;
            foreach (XmlElement ruleElement in element.GetElementsByTagName("logger"))
            {
                AppenderRule rule = new AppenderRule();
                string namePattern = ruleElement.GetAttribute("name");
                string appendTo = ruleElement.GetAttribute("appendTo");

                rule.LoggerNamePattern = namePattern;
                foreach (string appenderName in appendTo.Split(','))
                {
                    rule.AppenderNames.Add(appenderName.Trim());
                }
                rule.Final = false;

                if (ruleElement.HasAttribute("final"))
                {
                    rule.Final = true;
                }

                if (ruleElement.HasAttribute("level"))
                {
                    LogLevel level = LogLevelFromString(ruleElement.GetAttribute("level"));
                    rule.EnableLoggingForLevel(level);
                }
                else if (ruleElement.HasAttribute("levels"))
                {
                    string levelsString = ruleElement.GetAttribute("levels");
                    levelsString = CleanWhitespace(levelsString);

                    string[] tokens = levelsString.Split(',');
                    foreach (string s in tokens)
                    {
                        LogLevel level = LogLevelFromString(s);
                        rule.EnableLoggingForLevel(level);
                    }
                }
                else
                {
                    int minLevel = 0;
                    int maxLevel = (int)LogLevel.MaxLevel;

                    if (ruleElement.HasAttribute("minlevel"))
                    {
                        minLevel = (int)LogLevelFromString(ruleElement.GetAttribute("minlevel"));
                    }

                    if (ruleElement.HasAttribute("maxlevel"))
                    {
                        maxLevel = (int)LogLevelFromString(ruleElement.GetAttribute("maxlevel"));
                    }

                    for (int i = minLevel; i <= maxLevel; ++i)
                    {
                        rule.EnableLoggingForLevel((LogLevel)i);
                    }
                }

                AppenderRules.Add(rule);
            }
        }

        private static void AddLayoutAppendersFromElement(XmlElement element)
        {
            if (element == null)
                return;

            foreach (XmlElement appenderElement in element.GetElementsByTagName("add"))
            {
                string name = appenderElement.GetAttribute("name");
                string type = appenderElement.GetAttribute("type");
                string assemblyFile = appenderElement.GetAttribute("assemblyFile");

                if (assemblyFile != null && assemblyFile.Length > 0)
                {
                    Assembly asm = Assembly.LoadFrom(assemblyFile);
                    LayoutAppenderFactory.AddLayoutAppendersFromAssembly(asm);
                    continue;
                };

                string assemblyPartialName = appenderElement.GetAttribute("assemblyPartialName");

                if (assemblyPartialName != null && assemblyPartialName.Length > 0)
                {
                    Assembly asm = Assembly.Load(assemblyPartialName);
                    if (asm != null)
                    {
                        LayoutAppenderFactory.AddLayoutAppendersFromAssembly(asm);
                    }
                    else
                    {
                        throw new ApplicationException("Assembly with partial name " + assemblyPartialName + " not found.");
                    }
                    continue;
                };

                string assemblyName = appenderElement.GetAttribute("assembly");

                if (assemblyName != null && assemblyName.Length > 0)
                {
                    Assembly asm = Assembly.Load(assemblyName);
                    LayoutAppenderFactory.AddLayoutAppendersFromAssembly(asm);
                    continue;
                };

                LayoutAppenderFactory.AddLayoutAppender(name, Type.GetType(type));
            }
        }

        private void ConfigureAppendersFromElement(XmlElement element)
        {
            if (element == null)
                return;

            foreach (XmlElement appenderElement in element.GetElementsByTagName("appender"))
            {
                string type = appenderElement.GetAttribute("type");
                Appender newAppender = Appender.Create(type);

                ConfigureAppenderFromXmlElement(newAppender, appenderElement);
                AddAppender(newAppender.Name, newAppender);
            }
        }

        private void ConfigureAppenderFromXmlElement(Appender appender, XmlElement element)
        {
            Type appenderType = appender.GetType();

            foreach (XmlAttribute attrib in element.Attributes)
            {
                string name = attrib.LocalName;
                string value = attrib.InnerText;

                if (name == "type")
                    continue;

                PropertyHelper.SetPropertyFromString(appender, name, value);
            }

            foreach (XmlNode node in element.ChildNodes)
            {
                if (node is XmlElement)
                {
                    XmlElement el = (XmlElement)node;
                    string name = el.Name;
                    string value = el.InnerXml;

                    PropertyHelper.SetPropertyFromString(appender, name, value);
                }
            }
        }
    }
}
