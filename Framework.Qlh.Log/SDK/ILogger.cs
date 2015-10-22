using System;

namespace Framework.Qlh.Log
{
    public abstract class Logger
    {
        protected abstract void Write(LogLevel level, IFormatProvider formatProvider, string message, object[] args);

        public abstract bool IsEnabled(LogLevel level);
        public abstract bool IsDebugEnabled { get; }
        public abstract bool IsInfoEnabled { get; }
        public abstract bool IsWarnEnabled { get; }
        public abstract bool IsErrorEnabled { get; }
        public abstract bool IsFatalEnabled { get; }

        // make sure that each of the following methods does nothing but calling Write()
        // StackTrace functionality depends on it

        public void Log(LogLevel level, IFormatProvider formatProvider, string message, params object[] args)
        {
            Write(level, formatProvider, message, args);
        }

        public void Log(LogLevel level, string message, params object[] args)
        {
            Write(level, null, message, args);
        }

        public void Log(LogLevel level, string message)
        {
            Write(level, null, message, null);
        }

        public void Debug(IFormatProvider formatProvider, string message, params object[] args)
        {
            Write(LogLevel.Debug, formatProvider, message, args);
        }

        public void Debug(string message, params object[] args)
        {
            Write(LogLevel.Debug, null, message, args);
        }

        public void Debug(string message)
        {
            Write(LogLevel.Debug, null, message, null);
        }

        public void Info(IFormatProvider formatProvider, string message, params object[] args)
        {
            Write(LogLevel.Info, formatProvider, message, args);
        }

        public void Info(string message, params object[] args)
        {
            Write(LogLevel.Info, null, message, args);
        }

        public void Info(string message)
        {
            Write(LogLevel.Info, null, message, null);
        }

        public void Warn(IFormatProvider formatProvider, string message, params object[] args)
        {
            Write(LogLevel.Warn, formatProvider, message, args);
        }

        public void Warn(string message, params object[] args)
        {
            Write(LogLevel.Warn, null, message, args);
        }

        public void Warn(string message)
        {
            Write(LogLevel.Warn, null, message, null);
        }

        public void Error(IFormatProvider formatProvider, string message, params object[] args)
        {
            Write(LogLevel.Error, formatProvider, message, args);
        }

        public void Error(string message, params object[] args)
        {
            Write(LogLevel.Error, null, message, args);
        }

        public void Error(string message)
        {
            Write(LogLevel.Error, null, message, null);
        }

        public void Fatal(IFormatProvider formatProvider, string message, params object[] args)
        {
            Write(LogLevel.Fatal, formatProvider, message, args);
        }

        public void Fatal(string message, params object[] args)
        {
            Write(LogLevel.Fatal, null, message, args);
        }

        public void Fatal(string message)
        {
            Write(LogLevel.Fatal, null, message, null);
        }
    }
}