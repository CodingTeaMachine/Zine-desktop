using System.Diagnostics;
using MudBlazor;
using Zine.App.Logger;

namespace Zine.App.Exceptions;

public class HandledAppException : Exception
{
	public Severity Severity { get; init; }

	public HandledAppException(string message, Severity severity = Severity.Warning) : base(message)
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

		StackTrace stackTrace = new StackTrace(true);

		if (stackTrace.FrameCount <= 0) return;

		StackFrame frame = stackTrace.GetFrame(0)!;

		string messageToLog = $"HandledAppException: {message}" + Environment.NewLine
		                                                        + $"At: {frame.GetFileName()}:{frame.GetFileLineNumber()} @ {frame.GetMethod()}";

		SerilogLogger.Instance().FromSeverity(messageToLog, severity);
	}
}
