using System.Data;
using Microsoft.EntityFrameworkCore;
using Zine.App.Database;

namespace Zine.App.Domain.Settings;

public class SettingsContextFactory(IDbContextFactory<ZineDbContext> contextFactory) : ISettingsRepository
{
    public IEnumerable<Setting> GetAll()
    {
        //TODO: Error handling

        using var context = contextFactory.CreateDbContext();
        return context.Settings.ToList();
    }

    public Setting? GetByKey(string key)
    {
        //TODO: Error handling

        using var context = contextFactory.CreateDbContext();
        return context.Settings.FirstOrDefault(setting => setting.Key.Equals(key));
    }

    public bool SetSetting(string key, string value)
    {
        var setting = GetByKey(key);
        if (setting == null)
        {
            throw new DataException($"Key doesn't exist: {key}");
        }

        //TODO: Error handling

        setting.Value = value;
        using var context = contextFactory.CreateDbContext();
        int changedLines = context.SaveChanges();
        return changedLines == 1;
    }
}
