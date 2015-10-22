using Framework.Qlh.Log.Loggers;

namespace Framework.Qlh.Log
{
    public class Factory
    {
        private LogSetting _setting;

        public Factory(LogSetting setting)
        {
            _setting = setting;

            QueueStorage.Init(setting.AppLog, setting.SysLog, setting.AppNo);
        }

        public IBaseLogger CreateAppLogger()
        {
            return new AppLogger(_setting.AppLog, _setting.AppNo);
        }

        public IBaseLogger CreateSysLogger()
        {
            return new SysLogger(_setting.SysLog, _setting.AppNo);
        }

    }
}
