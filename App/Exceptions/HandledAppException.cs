using System.Diagnostics;
using MudBlazor;
using Zine.App.Logger;

namespace Zine.App.Exceptions;

public class HandledAppException : Exception
{
	public Severity Severity { get; init; }

	public HandledAppException(string message, Severity severity) : base(message)
	{
		Severity = severity;
		LogException(message, severity);
	}

	public HandledAppException(string message, Severity severity, Exception originalException) : base(message)
	{
		Severity = severity;
		LogException(originalException.Message, severity);
	}

	public HandledAppException(string message, Severity severity, string logMessage) : base(message)
	{
		LogException(logMessage, severity);
	}

	private void LogException(string message, Severity severity)
	{
		SerilogLogger.Instance()
			.SetIsPreFormattedMessageFroSingleLog(true)
			.FromSeverity($"{severity}: {message} {GetCalledLineFromLastFrame()}", severity);
	}

	private string GetCalledLineFromLastFrame()
	{
		StackTrace stackTrace = new StackTrace(true);

		if (stackTrace.FrameCount <= 0) return "";

		StackFrame? frame = stackTrace.GetFrames()
			.FirstOrDefault(frame => frame.GetFileName() != null && !frame.GetFileName()!.EndsWith(GetType().Name + ".cs"));

		if(frame == null) return "";

		return Environment.NewLine  + $"\t At: {frame.GetFileName()}:{frame.GetFileLineNumber()} {frame.GetMethod()}";
	}
}
