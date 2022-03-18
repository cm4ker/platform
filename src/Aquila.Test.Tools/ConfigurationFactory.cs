namespace Aquila.Test.Tools
{
    /// <summary>
    /// Пример конфигурации
    /// </summary>
    public static class ConfigurationFactory
    {
        public static string GetDatabaseConnectionString() => "Host=db1; Username=user; Password=password;";
    }
}