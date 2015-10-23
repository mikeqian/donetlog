using System;
using System.Collections;
using System.IO;
using System.Reflection;
using Framework.Qlh.Log.Config;

namespace Framework.Qlh.Log
{
    public sealed class LogManager
    {
        private static Hashtable _loggerCache = new Hashtable();
        private static LoggingConfiguration _config;
        private static bool _configLoaded = false;

        internal static bool ReloadConfigOnNextLog { get; set; }

        private LogManager()
        {
        }

        public static Logger GetLogger(string name)
        {
            if (ReloadConfigOnNextLog)
                ReloadConfig();

            lock (typeof(LogManager))
            {

                object l = _loggerCache[name];
                if (l != null)
                    return (Logger)l;

                ArrayList[] appendersByLevel = GetAppendersByLevelForLogger(name, Configuration);

                Logger newLogger = new LoggerImpl(name, appendersByLevel);
                _loggerCache[name] = newLogger;
                return newLogger;
            }
        }

        public static LoggingConfiguration Configuration
        {
            get
            {
                lock (typeof(LogManager))
                {
                    if (_configLoaded)
                        return _config;

                    if (_config == null)
                    {
                        // try to load default configuration
                        _config = XmlLoggingConfiguration.AppConfig;
                    }
                    if (_config == null)
                    {
                        string configFile = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;
                        configFile = configFile.Replace(".config", ".mlog");
                        if (File.Exists(configFile))
                        {
                            // Console.WriteLine("Attempting to load config from {0}", configFile);
                            _config = new XmlLoggingConfiguration(configFile);
                        }
                    }
                    if (_config == null)
                    {
                        Assembly nlogAssembly = typeof(LoggingConfiguration).Assembly;
                        if (!nlogAssembly.GlobalAssemblyCache)
                        {
                            string configFile = nlogAssembly.Location + ".mlog";
                            if (File.Exists(configFile))
                            {
                                // Console.WriteLine("Attempting to load config from {0}", configFile);
                                _config = new XmlLoggingConfiguration(configFile);
                            }
                        }
                    }

                    _configLoaded = true;
                    if (_config != null)
                    {
                        _watcher.Watch(_config.FileNamesToWatch);
                    }
                    return _config;
                }
            }

            set
            {
                _watcher.StopWatching();

                lock (typeof(LogManager))
                {
                    _config = value;
                    _configLoaded = true;

                    if (_config != null)
                    {
                        ReconfigExistingLoggers(_config);
                        _watcher.Watch(_config.FileNamesToWatch);
                    }
                }
            }
        }

        private static MultiFileWatcher _watcher = new MultiFileWatcher(new EventHandler(ConfigFileChanged));

        static LogManager()
        {
            ReloadConfigOnNextLog = false;
        }

        private static void ConfigFileChanged(object sender, EventArgs args)
        {
            // Console.WriteLine("ConfigFileChanged!!!");
            ReloadConfigOnNextLog = true;
        }

        public static void ClearLoggerCache()
        {
        }

        internal static void ReloadConfig()
        {
            lock (typeof(LogManager))
            {
                if (!ReloadConfigOnNextLog)
                    return;

                // Console.WriteLine("ReloadConfig: {0}", ReloadConfigOnNextLog);
                LoggingConfiguration newConfig = Configuration.Reload();
                if (newConfig != null)
                    Configuration = newConfig;
                ReloadConfigOnNextLog = false;
            }
        }

        internal static void ReconfigExistingLoggers(LoggingConfiguration config)
        {
            foreach (LoggerImpl logger in _loggerCache.Values)
            {
                logger.Reconfig(GetAppendersByLevelForLogger(logger.Name, config));
            }
        }

        internal static ArrayList[] GetAppendersByLevelForLogger(string name, LoggingConfiguration config)
        {
            ArrayList[] appendersByLevel = new ArrayList[(int)LogLevel.MaxLevel + 1];
            for (int i = 0; i < appendersByLevel.Length; ++i)
            {
                appendersByLevel[i] = new ArrayList();
            }

            if (config != null)
            {
                foreach (AppenderRule rule in config.AppenderRules)
                {
                    if (rule.Appenders.Count == 0)
                        continue;

                    if (rule.Matches(name))
                    {
                        for (int i = 0; i <= (int)LogLevel.MaxLevel; ++i)
                        {
                            if (rule.IsLoggingEnabledForLevel((LogLevel)i))
                            {
                                foreach (Appender appender in rule.Appenders)
                                {
                                    appendersByLevel[i].Add(appender);
                                }
                            }
                        }
                        if (rule.Final)
                            break;
                    }
                }
            }
            return appendersByLevel;
        }
    }
}
