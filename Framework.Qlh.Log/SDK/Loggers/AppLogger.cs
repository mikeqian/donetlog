using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Framework.Qlh.Common.Infrastructure.Server;
using Framework.Qlh.Log.Entity.Log;
using System;

namespace Framework.Qlh.Log.Loggers
{
    internal class AppLogger : IBaseLogger
    {
        private AppLogSetting _setting;
        private string _appNo;
        private static HttpClient _httpClient = null;

        static AppLogger()
        {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Connection.Add("keep-alive");
            _httpClient.Timeout = TimeSpan.FromSeconds(15);
        }

        public AppLogger(AppLogSetting setting, string appNo)
        {
            _setting = setting;
            _appNo = appNo;

            _httpClient.DefaultRequestHeaders.Add("User-Agent", appNo);
        }

        public void Info(string content)
        {
            this.WriteLog(null, string.Empty, content);
        }

        public void Error(string content)
        {
            this.WriteLog(null, string.Empty, content);
        }

        public void Warning(string content)
        {
            this.WriteLog(null, string.Empty, content);
        }

        public void Debug(string content)
        {
            this.WriteLog(null, string.Empty, content);
        }

        private void WriteLog(string ip, string logType, string content)
        {
            var log = new AppLog()
            {
                AppNo = _appNo,
                LogType = logType,
                Ip = ip,
                Content = content,
                CreateTime = DateTime.Now
            };

            var logs = new List<AppLog> { log };
            _httpClient.PostAsync(_setting.ApiUrl, new StringContent(JsonConvert.SerializeObject(logs), Encoding.UTF8, "application/json"));
        }

        public void Dispose() { }
        public void Flush() { }
    }
}
