using System;
using System.Text;

namespace Framework.Qlh.Log.LayoutAppenders
{
    [LayoutAppender("processtime")]
    public class ProcessTimeLayoutAppender : LayoutAppender
    {
        public override int GetEstimatedBufferSize(LogEventInfo ev)
        {
            return 32;
        }
        
        public override void Append(StringBuilder builder, LogEventInfo ev)
        {
            TimeSpan ts = ev.TimeStamp - LogEventInfo.ZeroDate;
            if (ts.Hours < 10)
                builder.Append('0');
            builder.Append(ts.Hours);
            builder.Append(':');
            if (ts.Minutes < 10)
                builder.Append('0');
            builder.Append(ts.Minutes);
            builder.Append(':');
            if (ts.Seconds < 10)
                builder.Append('0');
            builder.Append(ts.Seconds);
            builder.Append(':');
            if (ts.Milliseconds < 1000)
                builder.Append('0');
            if (ts.Milliseconds < 100)
                builder.Append('0');
            if (ts.Milliseconds < 10)
                builder.Append('0');
            builder.Append(ts.Milliseconds);
        }
    }
}
