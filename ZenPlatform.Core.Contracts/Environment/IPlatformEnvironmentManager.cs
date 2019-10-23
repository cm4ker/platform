using System.Collections.Generic;
using ZenPlatform.Core.Environment.Contracts;
using ZenPlatform.QueryBuilder;

namespace ZenPlatform.Core.Environment
{
    /// <summary>
    /// Менеджер среды 
    /// </summary>
    public interface IPlatformEnvironmentManager
    {
        IEnvironment GetEnvironment(string name);

        void AddWorkEnvironment(IStartupConfig config);

        List<IEnvironment> GetEnvironmentList();
    }

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