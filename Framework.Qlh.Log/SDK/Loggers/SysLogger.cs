using System.Dynamic;
using Framework.Qlh.Log.Entity.Log;
using System;

namespace Framework.Qlh.Log.Loggers
{
    internal class SysLogger
    {

        #region Fields

        private string _AppNo;

        private SysLogSetting _Setting;

        #endregion

        #region Constructors

        public SysLogger(SysLogSetting setting, string appNo)
        {
            _AppNo = appNo;
            _Setting = setting;
        }

        #endregion

        public void Error(string content)
        {
            this.WriteLog(null, LogLevel.Error, content);
        }

        public void Info(string message)
        {
            this.WriteLog(null, LogLevel.Info, message);
        }

        public void Debug(string message)
        {
            this.WriteLog(null, LogLevel.Debug, message);
        }

        public void Warning(string message)
        {
            this.WriteLog(null, LogLevel.Warn, message);
        }

        #region Help Methods

        private void WriteLog(string ip, LogLevel level, string message)
        {
            var log = new SysLog()
            {
                AppNo = _AppNo,
                Level = level,
                Ip = ip,
                Message = message,
                CreateTime = DateTime.Now
            };

        }

        #endregion

        public void Dispose()
        {
        }
    }
}
