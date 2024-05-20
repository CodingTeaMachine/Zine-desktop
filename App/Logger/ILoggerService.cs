namespace Zine.App.Logger;

public interface ILoggerService
{
    public void Information(string message);
    public void Warning(string message);
    public void Error(string message);
}
