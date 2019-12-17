﻿using ZenPlatform.Compiler;
using ZenPlatform.Compiler.Contracts;
using ZenPlatform.QueryBuilder;

namespace ZenPlatform.Configuration.Contracts.Data
{
    public interface IXCCompiller
    {
        IAssembly Build(IXCRoot configuration, CompilationMode mode, SqlDatabaseType targetDatabaseType);
    }
}
