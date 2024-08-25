using MudBlazor;
using Serilog;
using Zine.App.Enums;
using Zine.App.Helpers;
using ILogger = Serilog.Core.Logger;

namespace Zine.App.Logger;

public class SerilogLogger : ILoggerService
{
    private readonly ILogger _logger;

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
        _logger.Information(message);
    }

    public void Warning(string message)
    {
        _logger.Warning(message);
    }

    public void Error(string message)
    {
        _logger.Error(message);
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
}
