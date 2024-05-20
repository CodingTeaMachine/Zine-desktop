using Zine.App.Model.DB;

namespace Zine.App.Repositories;

public interface ISettingsRepository : IRepository<Setting>
{
    public Setting? GetByKey(string key);

    public bool SetSetting(string key, string value);
}
