using System.Data;
using Zine.App.Logger;
using Zine.App.Model.DB;
using Zine.App.Repositories;

namespace Zine.App.Services;

public class SettingsService(ISettingsRepository repository, ILoggerService logger) : ISettingsService
{
    public IRepository<Setting> Repository { get; } = repository;

    public IEnumerable<Setting> GetAll()
    {
        return repository.GetAll();
    }

    public Setting? GetByKey(string key)
    {
        return repository.GetByKey(key);
    }

    public bool SetSetting(string key, string value)
    {
        try
        {
            return repository.SetSetting(key, value);
        }
        catch (DataException e)
        {
            logger.Error(e.Message);
            return false;
        }
    }
}
