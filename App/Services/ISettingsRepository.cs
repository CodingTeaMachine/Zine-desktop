using Zine.App.Model.DB;

namespace Zine.App.Services;

public interface ISettingsService : IService<Setting>
{
    public Setting? GetByKey(string key);

    public bool SetSetting(string key, string value);
}
