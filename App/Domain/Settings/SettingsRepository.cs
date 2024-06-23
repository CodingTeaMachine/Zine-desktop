using System.Data;
using Microsoft.EntityFrameworkCore;
using Zine.App.Database;

namespace Zine.App.Domain.Settings;

public class SettingsRepository(IDbContextFactory<ZineDbContext> contextFactory) : Repository(contextFactory), ISettingsRepository
{
    public IEnumerable<Setting> GetAll()
    {
        return base.GetDbContext().Settings.ToList();
    }

    public Setting? GetByKey(string key)
    {
        return base.GetDbContext().Settings.FirstOrDefault(setting => setting.Key.Equals(key));
    }

    public bool SetSetting(string key, string value)
    {
        var setting = GetByKey(key);
        if (setting == null)
        {
            throw new DataException($"Key doesn't exist: {key}");
        }

        setting.Value = value;
        int changedLines = base.GetDbContext().SaveChanges();
        return changedLines == 1;
    }
}
