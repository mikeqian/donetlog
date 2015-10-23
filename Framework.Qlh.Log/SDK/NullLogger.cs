using System;

namespace Framework.Qlh.Log
{
    public sealed class NullLogger : Logger
    {
        protected override void Write(LogLevel level, IFormatProvider formatProvider, string message, object[] args) { }
        public override bool IsEnabled(LogLevel level) { return false; }
        public override bool IsDebugEnabled { get { return false; } }
        public override bool IsInfoEnabled { get { return false; } }
        public override bool IsWarnEnabled { get { return false; } }
        public override bool IsErrorEnabled { get { return false; } }
        public override bool IsFatalEnabled { get { return false; } }
    }
}
