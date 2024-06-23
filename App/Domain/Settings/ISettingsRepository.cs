namespace Zine.App.Domain.Settings;

public interface ISettingsRepository
{
    public IEnumerable<Setting> GetAll();
    public Setting? GetByKey(string key);

    public bool SetSetting(string key, string value);
}
