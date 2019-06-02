﻿using ZenPlatform.QueryBuilder;

namespace ZenPlatform.Data
{
    public interface IDataContextManager
    {
        DataContext GetContext();
        void Initialize(SqlDatabaseType dbType, string connectionString);

        SqlCompillerBase SqlCompiler { get; }
    }
}