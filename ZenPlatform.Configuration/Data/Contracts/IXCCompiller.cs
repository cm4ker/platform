﻿using ZenPlatform.Compiler;
using ZenPlatform.Compiler.Contracts;
using ZenPlatform.Configuration.Structure;
using ZenPlatform.QueryBuilder;

namespace ZenPlatform.Configuration.Data.Contracts
{
    public interface IXCCompiller
    {
        IAssembly Build(XCRoot configuration, CompilationMode mode, SqlDatabaseType targetDatabaseType);
    }
}
