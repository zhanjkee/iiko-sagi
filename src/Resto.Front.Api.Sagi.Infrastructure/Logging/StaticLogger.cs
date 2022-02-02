namespace Resto.Front.Api.Sagi.Infrastructure.Logging
{
	// NOTE: For debugging http request.
	public static class StaticLogger
	{
		public delegate void LogEvent(string message);

		public static LogEvent LogEventInfo { get; set; }
		public static LogEvent LogEventError { get; set; }
		public static LogEvent LogEventWarn { get; set; }

		public static void LogInfo(string message)
		{
			if (LogEventInfo == null)
				return;

			LogEventInfo(message);
		}

		public static void LogWarn(string message)
		{
			if (LogEventWarn == null)
				return;

			LogEventWarn(message);
		}

		public static void LogError(string message)
		{
			if (LogEventError == null)
				return;

			LogEventError(message);
		}
	}
}