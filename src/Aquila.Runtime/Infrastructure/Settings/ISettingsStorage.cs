namespace Aquila.Core.Infrastructure.Settings
{
    public interface ISettingsStorage
    {
        T Get<T>() where T : class, new();
        void Save();
    }
}