// 
using System;
using System.Collections;
using System.Collections.Specialized;
using System.Diagnostics;
using Framework.Qlh.Log;

namespace Framework.Qlh.Log.Appenders
{
    public class MemoryAppender : Appender
    {
		private ArrayList _logs = new ArrayList();

        public override void Append(LogEventInfo ev) {
			_logs.Add(CompiledLayout.GetFormattedMessage(ev));
        }

		public ArrayList Logs
		{
			get { return _logs; }
		}
    }
}
