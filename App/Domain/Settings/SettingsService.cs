using System.Data;
using Zine.App.Logger;

namespace Zine.App.Domain.Settings;

public class SettingsService(ISettingsRepository settingsRepository, ILoggerService logger) : ISettingsService
{

    public IEnumerable<Setting> GetAll()
    {
        return settingsRepository.GetAll();
    }

    public Setting? GetByKey(string key)
    {
        return settingsRepository.GetByKey(key);
    }

    public bool SetSetting(string key, string value)
    {
        try
        {
            return settingsRepository.SetSetting(key, value);
        }
        catch (DataException e)
        {
            logger.Error(e.Message);
            return false;
        }
    }
}
