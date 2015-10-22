// 
// Copyright (c) 2004 Jaroslaw Kowalski <jaak@polbox.com>
// 
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without 
// modification, are permitted provided that the following conditions 
// are met:
// 
// * Redistributions of source code must retain the above copyright notice, 
//   this list of conditions and the following disclaimer. 
// 
// * Redistributions in binary form must reproduce the above copyright notice,
//   this list of conditions and the following disclaimer in the documentation
//   and/or other materials provided with the distribution. 
// 
// * Neither the name of the Jaroslaw Kowalski nor the names of its 
//   contributors may be used to endorse or promote products derived from this
//   software without specific prior written permission. 
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE 
// IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE 
// ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE 
// LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR 
// CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF
// SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS 
// INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN 
// CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) 
// ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF 
// THE POSSIBILITY OF SUCH DAMAGE.
// 

using System;
using System.Collections;
using System.Xml;
using System.IO;
using System.Reflection;
using Framework.Qlh.Log;
using NLog.Config;

namespace NLog
{
    public sealed class LogManager 
    {
        private static Hashtable _loggerCache = new Hashtable();
        private static LoggingConfiguration _config;
        private static bool _configLoaded = false;
        private static bool _reloadConfigOnNextLog = false;

        internal static bool ReloadConfigOnNextLog
        {
            get { return _reloadConfigOnNextLog; }
            set { _reloadConfigOnNextLog = value; }
        }

        private LogManager()
        {
        }

        public static Logger GetLogger(string name) {
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
            get { 
                lock (typeof(LogManager)) {
                    if (_configLoaded)
                        return _config;

#if !NETCF
                    if (_config == null) 
                    {
                        // try to load default configuration
                        _config = XmlLoggingConfiguration.AppConfig;
                    }
                    if (_config == null) {
                        string configFile = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;
                        configFile = configFile.Replace(".config", ".nlog");
                        if (File.Exists(configFile)) {
                            // Console.WriteLine("Attempting to load config from {0}", configFile);
                            _config = new XmlLoggingConfiguration(configFile);
                        }
                    }
                    if (_config == null) {
                        Assembly nlogAssembly = typeof(LoggingConfiguration).Assembly;
                        if (!nlogAssembly.GlobalAssemblyCache) {
                            string configFile = nlogAssembly.Location + ".nlog";
                            if (File.Exists(configFile)) {
                                // Console.WriteLine("Attempting to load config from {0}", configFile);
                                _config = new XmlLoggingConfiguration(configFile);
                            }
                        }
                    }
#endif

                    _configLoaded = true;
                    if (_config != null) {
                        _watcher.Watch(_config.FileNamesToWatch);
                    }
                    return _config;
                }
            }

            set { 
                _watcher.StopWatching();

                lock (typeof(LogManager)) {
                    _config = value; 
                    _configLoaded = true;

                    if (_config != null) {
                        ReconfigExistingLoggers(_config);
                        _watcher.Watch(_config.FileNamesToWatch);
                    }
                }
            }
        }

        private static MultiFileWatcher _watcher = new MultiFileWatcher(new EventHandler(ConfigFileChanged));

        private static void ConfigFileChanged(object sender, EventArgs args) {
            // Console.WriteLine("ConfigFileChanged!!!");
            ReloadConfigOnNextLog = true;
        }

        public static void ClearLoggerCache() {
        }

        internal static void ReloadConfig() {
            lock (typeof(LogManager)) {
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
            foreach (LoggerImpl logger in _loggerCache.Values) {
                logger.Reconfig(GetAppendersByLevelForLogger(logger.Name, config));
            }
        }

        internal static ArrayList[] GetAppendersByLevelForLogger(string name, LoggingConfiguration config)
        {
            ArrayList[] appendersByLevel = new ArrayList[(int)LogLevel.MaxLevel + 1];
            for (int i = 0; i < appendersByLevel.Length; ++i) {
                appendersByLevel[i] = new ArrayList();
            }

            if (config != null) {
                foreach (AppenderRule rule in config.AppenderRules) {
                    if (rule.Appenders.Count == 0)
                        continue;

                    if (rule.Matches(name)) {
                        for (int i = 0; i <= (int)LogLevel.MaxLevel; ++i) {
                            if (rule.IsLoggingEnabledForLevel((LogLevel)i)) {
                                foreach (Appender appender in rule.Appenders) {
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
