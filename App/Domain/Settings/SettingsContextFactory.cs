using System.Data;
using Microsoft.EntityFrameworkCore;
using Zine.App.Database;

namespace Zine.App.Domain.Settings;

public class SettingsContextFactory(IDbContextFactory<ZineDbContext> contextFactory) : Repository(contextFactory), ISettingsRepository
{
    public IEnumerable<Setting> GetAll()
    {
        return GetDbContext().Settings.ToList();
    }

    public Setting? GetByKey(string key)
    {
        return GetDbContext().Settings.FirstOrDefault(setting => setting.Key.Equals(key));
    }

    public bool SetSetting(string key, string value)
    {
        var setting = GetByKey(key);
        if (setting == null)
        {
            throw new DataException($"Key doesn't exist: {key}");
        }

        setting.Value = value;
        int changedLines = GetDbContext().SaveChanges();
        return changedLines == 1;
    }
}
