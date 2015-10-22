using System;
using System.IO;

namespace Framework.Qlh.Log.Loggers
{
    public class LocalLogger
    {
        private static object _loggerLock = new object();

        private static void WriteFile(string message, string level)
        {
            try
            {
                message = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " " + message;
                message += Environment.NewLine;

                var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, DateTime.Now.ToString("yyyy-MM-dd") + "." + level);
                lock (_loggerLock)
                {
                    File.AppendAllText(path, message);
                }
            }
            catch (Exception exception)
            {
                if (exception.MustBeRethrown())
                {
                    throw;
                }

                // we have no place to log the message to so we ignore it
            }
        }

        public void Info(string content)
        {
            WriteFile(content, "info");
        }

        public void Error(string content)
        {
            WriteFile(content, "error");
        }

        public void Warning(string content)
        {
            WriteFile(content, "warn");
        }

        public void Trace(string content)
        {
            WriteFile(content, "trace");
        }

        public void Debug(string content)
        {
            WriteFile(content, "debug");
        }
    }
}
