using System.Diagnostics;
using MudBlazor;
using Serilog;
using Zine.App.Enums;
using Zine.App.Helpers;
using ILogger = Serilog.Core.Logger;

namespace Zine.App.Logger;

public class SerilogLogger : ILoggerService
{
    private readonly ILogger _logger;
    private bool _isPreFormattedMessage = false;

    public SerilogLogger(IConfiguration configuration)
    {
        var logFile = Path.Join(configuration.GetValue<string>(ConfigKeys.LogFolder), "log-.log");
        _logger = new LoggerConfiguration()
            .WriteTo.File(logFile, rollingInterval: RollingInterval.Day)
            .CreateLogger();
    }

    public static SerilogLogger Instance()
    {
        return new SerilogLogger(ConfigurationHelper.Config);
    }

    public void Information(string message)
    {
        message = _isPreFormattedMessage
                ? message
                : GetMessage(message);

        _logger.Information(message);

        _isPreFormattedMessage = false;
    }

    public void Warning(string message)
    {
        message = _isPreFormattedMessage
            ? message
            : GetMessage(message);

        _logger.Warning(message);

        _isPreFormattedMessage = false;
    }

    public void Error(string message)
    {
        message = _isPreFormattedMessage
            ? message
            : GetMessage(message);

        _logger.Error(message);

        _isPreFormattedMessage = false;
    }

    public void FromSeverity(string message, Severity severity)
    {
        switch (severity)
        {
            case Severity.Normal:
            case Severity.Info:
            case Severity.Success:
                Information(message);
                break;
            case Severity.Warning:
                Warning(message);
                break;
            case Severity.Error:
                Error(message);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(severity), severity, null);
        }
    }

    private string GetMessage(string message)
    {
        StackTrace stackTrace = new StackTrace(true);

        if (stackTrace.FrameCount <= 0)
            return message;

        StackFrame? frame = stackTrace.GetFrames()
            .FirstOrDefault(frame => frame.GetFileName() != null && !frame.GetFileName()!.EndsWith(GetType().Name + ".cs"));


        return frame != null
            ? $"{Path.GetFileNameWithoutExtension(frame.GetFileName())}.{frame.GetMethod()?.Name} : {message}"
            : message;
    }

    public SerilogLogger SetIsPreFormattedMessageFroSingleLog(bool isPreFormattedMessage)
    {
        _isPreFormattedMessage = isPreFormattedMessage;

        return this;
    }
}
