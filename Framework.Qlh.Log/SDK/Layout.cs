using System;
using System.Collections;
using System.Text;
using Framework.Qlh.Log.LayoutAppenders;

namespace Framework.Qlh.Log
{
    public class Layout
    {
        public Layout()
        {
            Text = String.Empty;
        }
        
        public Layout(string txt)
        {
            Text = txt;
        }

        private string _layoutText;
        private LayoutAppender[] _layoutAppenders;
		private int _needsStackTrace = 0;

        public string Text
        {
            get { return _layoutText; }
            set { 
                _layoutText = value; 
                _layoutAppenders = CompileLayout(_layoutText, out _needsStackTrace);
            }
        }

        public string GetFormattedMessage(LogEventInfo ev) {
            int size = 0;

            foreach (LayoutAppender app in _layoutAppenders) {
                int ebs = app.GetEstimatedBufferSize(ev);
                size += ebs;
            }
                
            StringBuilder builder = new StringBuilder(size);

            foreach (LayoutAppender app in _layoutAppenders) {
                app.Append(builder, ev);
            }

            return builder.ToString();
        }

        private static LayoutAppender[] CompileLayout(string s, out int needsStackTrace) {
            ArrayList result = new ArrayList();
			needsStackTrace = 0;

            int startingPos = 0;
            int pos = s.IndexOf("${", startingPos);

            while (pos >= 0) {
                if (pos != startingPos) {
                    result.Add(new LiteralLayoutAppender(s.Substring(startingPos, pos - startingPos)));
                }
                int pos2 = s.IndexOf("}", pos + 2);
                if (pos2 >= 0) {
                    startingPos = pos2 + 1;
                    string item = s.Substring(pos + 2, pos2 - pos - 2);
                    int paramPos = item.IndexOf(':');
                    string layoutAppenderName = item;
                    string layoutAppenderParams = null;
                    if (paramPos >= 0) {
                        layoutAppenderParams = layoutAppenderName.Substring(paramPos + 1);
                        layoutAppenderName = layoutAppenderName.Substring(0, paramPos);
                    }

                    LayoutAppender newLayoutAppender = LayoutAppenderFactory.CreateLayoutAppender(layoutAppenderName, layoutAppenderParams);
					int nst = newLayoutAppender.NeedsStackTrace();
                    if (nst > needsStackTrace)
						needsStackTrace = nst;

                    result.Add(newLayoutAppender);
                    pos = s.IndexOf("${", startingPos);
                } else {
                    break;
                }
            }
            if (startingPos != s.Length) {
                result.Add(new LiteralLayoutAppender(s.Substring(startingPos, s.Length - startingPos)));
            }

            return (LayoutAppender[])result.ToArray(typeof(LayoutAppender));
        }

        public int NeedsStackTrace()
        {
            return _needsStackTrace;
        }
    }
}
