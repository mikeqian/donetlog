using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using Framework.Qlh.Common.Infrastructure.Server;
using Framework.Qlh.Log.Entity.Log;
using System.Net.Http;

namespace Framework.Qlh.Log.Loggers
{
    internal static class QueueStorage
    {
        private static ConcurrentQueue<SysLog> _sysLogQueue = new ConcurrentQueue<SysLog>();
        private static HttpClient _httpClient = new HttpClient();

        private static SysLogSetting _sysLogSetting;
        private static DateTime _sysLogLastFlushTime;

        static QueueStorage()
        {
            Disposed = false;
        }

        private static ConcurrentQueue<SysLog> SysLogQueue
        {
            get
            {
                return _sysLogQueue;
            }
        }

        public static void Init(AppLogSetting appLogSetting, SysLogSetting syslogSetting, string appNo)
        {
            _sysLogSetting = syslogSetting;
            _sysLogLastFlushTime = DateTime.Now;

            InitSysLogTask();

            _httpClient.DefaultRequestHeaders.Add("User-Agent", appNo);
        }

        public static bool Disposed { get; set; }

        public static void EnqueueSysLog(SysLog log)
        {
            SysLogQueue.Enqueue(log);
        }

        public static void FlushAllSysLog()
        {
            FlushAllLogs();
        }

        public static void FlushSysLog(SysLog log)
        {
            var logs = new List<SysLog> { log };
            PushSysLogs(logs);
        }

        private static void InitSysLogTask()
        {
            Task.Run(() =>
            {
                while (!Disposed)
                {
                    try
                    {
                        _sysLogLastFlushTime = PushLogByPeriod(_sysLogLastFlushTime, _sysLogSetting.AutoFlushPeriod);

                        PushLogBySize(_sysLogSetting.MaxQueueNum);
                    }
                    catch (Exception e)
                    {
                        new LocalLogger().Error(e.ToString());
                    }

                    Thread.Sleep(1000);
                }
            });
        }

        private static DateTime PushLogByPeriod(DateTime lastFlushTime, int second)
        {
            var newFlushTime = lastFlushTime;

            if (lastFlushTime.AddSeconds(second) < DateTime.Now)
            {
                FlushAllLogs();
                newFlushTime = DateTime.Now;
            }

            return newFlushTime;
        }

        private static void FlushAllLogs()
        {
            var logs = new List<SysLog>();

            while (true)
            {
                SysLog log;
                if (_sysLogQueue.TryDequeue(out log))
                {
                    logs.Add(log);
                }
                else
                {
                    break;
                }
            }

            if (logs.Count > 0)
            {
                PushSysLogs(logs);
            }
        }

        private static void PushLogBySize(int maxQueueNum)
        {
            var results = new List<SysLog>();
            if (maxQueueNum <= _sysLogQueue.Count)
            {
                for (var i = 0; i < _sysLogSetting.MaxQueueNum; i++)
                {
                    SysLog log;
                    _sysLogQueue.TryDequeue(out log);
                    results.Add(log);
                }

                PushSysLogs(results);
            }
        }

        private static void PushSysLogs(List<SysLog> logs)
        {
            if (logs == null || logs.Count == 0)
            {
                return;
            }

            var content = new StringContent(JsonConvert.SerializeObject(logs), Encoding.UTF8, "application/json");
            _httpClient.PostAsync(_sysLogSetting.ApiUrl, content);
        }

    }
}
