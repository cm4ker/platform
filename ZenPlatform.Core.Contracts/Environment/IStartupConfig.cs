using ZenPlatform.QueryBuilder;

namespace ZenPlatform.Core.Contracts.Environment
{
    public interface IStartupConfig
    {
        /// <summary>
        /// Строка подключения к базе
        /// </summary>
        string ConnectionString { get; set; }

        /// <summary>
        /// Тип базы данных, которую будет обслуживать рабочий процесс
        /// </summary>
        SqlDatabaseType DatabaseType { get; set; }
    }
}