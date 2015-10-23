using System;
using System.Collections;

namespace Framework.Qlh.Log.Config
{
    public class AppenderRule
    {
		internal enum MatchMode
		{
			All,
			None,
			Equals,
			StartsWith,
			EndsWith,
			Contains,
		}

        private string _loggerNamePattern;
		private MatchMode _loggerNameMatchMode;
		private string _loggerNameMatchArgument;

		private bool[] _logLevels = new bool[(int)LogLevel.MaxLevel + 1];
        private ArrayList _appenderNames = new ArrayList();
        private ArrayList _appenders = new ArrayList();
        private bool _final = false;

		public ArrayList AppenderNames
		{
			get { return _appenderNames; }
		}

		public ArrayList Appenders
		{
			get { return _appenders; }
		}

		public bool Final
		{
			get { return _final; }
			set { _final = value; }
		}

        public AppenderRule()
        {
        }

		public AppenderRule(string loggerNamePattern, string appenderName, params LogLevel[] levels)
		{
			_loggerNamePattern = loggerNamePattern;
			_appenderNames.Add(appenderName);
			foreach (LogLevel ll in levels)
			{
				_logLevels[(int)ll] = true;
			}
		}

		public void EnableLoggingForLevel(LogLevel level)
		{
			_logLevels[(int)level] = true;
		}

		public void DisableLoggingForLevel(LogLevel level)
		{
			_logLevels[(int)level] = false;
		}

		public bool IsLoggingEnabledForLevel(LogLevel level)
		{
			return _logLevels[(int)level];
		}

		public string LoggerNamePattern
		{
			get { return _loggerNamePattern; }
			set 
			{
				_loggerNamePattern = value;
				int firstPos = _loggerNamePattern.IndexOf('*');
				int lastPos = _loggerNamePattern.LastIndexOf('*');

				if (firstPos < 0)
				{
					_loggerNameMatchMode = MatchMode.Equals;
					_loggerNameMatchArgument = value;
					return;
				}

				if (firstPos == lastPos)
				{
					string before = LoggerNamePattern.Substring(0, firstPos);
					string after = LoggerNamePattern.Substring(firstPos + 1);

					if (before.Length > 0) 
					{
						_loggerNameMatchMode = MatchMode.StartsWith;
						_loggerNameMatchArgument = before;
						return;
					}
                    
					if (after.Length > 0) 
					{
						_loggerNameMatchMode = MatchMode.EndsWith;
						_loggerNameMatchArgument = after;
						return;
					}
					return;
				}

				// *text*
				if (firstPos == 0 && lastPos == LoggerNamePattern.Length - 1) 
				{
					string text = LoggerNamePattern.Substring(1, LoggerNamePattern.Length - 2);
					_loggerNameMatchMode = MatchMode.Contains;
					_loggerNameMatchArgument = text;
					return;
				}

				_loggerNameMatchMode = MatchMode.None;
				_loggerNameMatchArgument = String.Empty;
			}
		}

		public bool Matches(string loggerName) 
		{
			switch (_loggerNameMatchMode)
			{
				case MatchMode.All:
					return true;

				default:
				case MatchMode.None:
					return false;

				case MatchMode.Equals:
					return String.CompareOrdinal(loggerName, _loggerNameMatchArgument) == 0;

				case MatchMode.StartsWith:
					return loggerName.StartsWith(_loggerNameMatchArgument);

				case MatchMode.EndsWith:
					return loggerName.EndsWith(_loggerNameMatchArgument);

				case MatchMode.Contains:
					return loggerName.IndexOf(_loggerNameMatchArgument) >= 0;
			}
        }

        public void Resolve(LoggingConfiguration configuration)
        {
            foreach (string s in AppenderNames) {
                Appender app = configuration.FindAppenderByName(s);

                if (app != null) {
                    Appenders.Add(app);
                }
            }
        }
    }
}
