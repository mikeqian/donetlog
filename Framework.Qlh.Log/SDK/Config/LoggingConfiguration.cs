using System.Collections;

namespace Framework.Qlh.Log.Config
{
    public class LoggingConfiguration : ConfigSectionHandler
    {
        private Hashtable _appenders = new Hashtable();
        private ArrayList _appenderRules = new ArrayList();

        public LoggingConfiguration() { }

		public void AddAppender(string name, Appender appender) 
		{
			_appenders[name] = appender;
		}

		public void AddAppenderRule(AppenderRule rule) 
		{
			_appenderRules.Add(rule);
		}

        public Appender FindAppenderByName(string name) {
            return (Appender)_appenders[name];
        }

		public ArrayList AppenderRules
		{
			get { return _appenderRules; }
		}

		// implementation details

        public void ResolveAppenders()
        {
            foreach (AppenderRule rule in _appenderRules) {
                rule.Resolve(this);
            }
        }

        public virtual ICollection FileNamesToWatch
        {
            get { return null; }
        }

        public virtual LoggingConfiguration Reload()
        {
            return null;
        }
    }
}
