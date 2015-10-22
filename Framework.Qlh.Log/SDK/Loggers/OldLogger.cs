using Framework.Qlh.Log.Entity;
using System;
using System.Configuration;

namespace Framework.Qlh.Log.Loggers
{
    public sealed class OldLogger
    {
        private static OldLogger _oldLogger = new OldLogger();
        private IBaseLogger _appLogger;
        private IBaseLogger _sysLogger;
        private Factory factory;

        OldLogger()
        {
            var setting = ConfigurationManager.GetSection("LogSetting") as LogSetting;
            if (setting == null)
            {
                var exception = new Exception("请先配置LogSetting节点");
                new LocalLogger().Error(exception.ToString());

                throw exception;
            }
            factory = new Factory(setting);

        }

        public static OldLogger Instance
        {
            get
            {
                return _oldLogger;
            }
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
