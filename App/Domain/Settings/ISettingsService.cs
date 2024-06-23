namespace Zine.App.Domain.Settings;

public interface ISettingsService
{
    public IEnumerable<Setting> GetAll();

    public Setting? GetByKey(string key);

    public bool SetSetting(string key, string value);
}
