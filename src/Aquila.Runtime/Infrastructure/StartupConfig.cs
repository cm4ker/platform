﻿using Aquila.Data;

namespace Aquila.Core
{
    /// <summary>
    /// Минимально необходимый набор параметров, чтобы всё заработало
    /// </summary>
    public class StartupConfig
    {
        /// <summary>
        /// Строка подключения к базе
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        /// Тип базы данных, которую будет обслуживать рабочий процесс
        /// </summary>
        public SqlDatabaseType DatabaseType { get; set; }
    }
}