using System.Data;
using Zine.App.Logger;
using Zine.App.Model.DB;
using Zine.App.Repositories;

namespace Zine.App.Services;

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
