using Zine.App.Model.DB;

namespace Zine.App.Repositories;

public interface ISettingsRepository
{
    public IEnumerable<Setting> GetAll();
    public Setting? GetByKey(string key);

    public bool SetSetting(string key, string value);
}
