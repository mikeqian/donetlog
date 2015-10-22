using Framework.Qlh.Log.Entity;
using System;
using System.Configuration;

namespace Framework.Qlh.Log.Loggers
{
    public sealed class Logger : IBaseLogger
    {
        private static Logger _logger = new Logger();
        private IBaseLogger _appLogger;
        private IBaseLogger _sysLogger;
        private Factory factory;

        Logger()
        {
            var setting = ConfigurationManager.GetSection("LogSetting") as LogSetting;
            if (setting == null)
            {
                var exception = new Exception("请先配置LogSetting节点");
                new LocalLogger().Error(exception.ToString());

                throw exception;
            }
            factory = new Factory(setting);
            _appLogger = factory.CreateAppLogger();
            _sysLogger = factory.CreateSysLogger();
            //_appTracer = factory.CreateAppTracer();
        }

        public static Logger Instance
        {
            get
            {
                return _logger;
            }
        }

        public void Info(string message)
        {
            _sysLogger.Info(message);
        }

        public void Warning(string message)
        {
            _sysLogger.Warning(message);
        }

        public void Error(string message)
        {
            _sysLogger.Error(message);
        }

        public void Error(Exception ex)
        {
            _sysLogger.Error(ex.ToString());
        }

        public void Debug(string message)
        {
            _sysLogger.Debug(message);
        }

        public void AppInfo(string content)
        {
            _appLogger.Info(content);
        }

        public void AppWarn(string content)
        {
            _appLogger.Warning(content);
        }

        public void AppError(string content)
        {
            _appLogger.Error(content);
        }

        public void Flush()
        {
            _appLogger.Flush();
            _sysLogger.Flush();
        }

        public void Dispose()
        {
            _appLogger.Dispose();
            _sysLogger.Dispose();
        }
    }
}
