using System;

namespace Framework.Qlh.Log.Appenders
{
    public class ConsoleAppender : Appender
    {
        public override void Append(LogEventInfo ev) {
            Console.WriteLine(CompiledLayout.GetFormattedMessage(ev));
        }
    }
}
