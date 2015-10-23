namespace Framework.Qlh.Log.Appenders
{
    public class NullAppender : Appender
    {
		private bool _formatMessage = false;

		public bool FormatMessage
		{
			get { return _formatMessage; }
			set { _formatMessage = value; }
		}

		public override void Append(LogEventInfo ev) 
		{
			if (_formatMessage)
			{
				CompiledLayout.GetFormattedMessage(ev);
			}
		}
    }
}
