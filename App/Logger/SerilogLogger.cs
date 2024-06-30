using Serilog;
using Zine.App.Enums;
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
}
