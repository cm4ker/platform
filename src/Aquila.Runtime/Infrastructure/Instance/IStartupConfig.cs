using Aquila.Data;
using Aquila.QueryBuilder;

namespace Aquila.Core.Contracts.Instance
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