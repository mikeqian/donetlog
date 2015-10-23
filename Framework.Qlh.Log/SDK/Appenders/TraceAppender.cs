using System.Diagnostics;

namespace Framework.Qlh.Log.Appenders
{
    public class TraceAppender : Appender
    {
        public override void Append(LogEventInfo ev) {
            if (ev.Level >= LogLevel.Error) {
                Trace.Fail(CompiledLayout.GetFormattedMessage(ev));
            } else {
                Trace.WriteLine(CompiledLayout.GetFormattedMessage(ev));
            }
        }
    }
}
