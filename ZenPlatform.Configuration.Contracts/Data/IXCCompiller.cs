using ZenPlatform.Compiler;
using ZenPlatform.Compiler.Contracts;
using ZenPlatform.Compiler.Roslyn.DnlibBackend;
using ZenPlatform.QueryBuilder;

namespace ZenPlatform.Configuration.Contracts.Data
{
    public interface IXCCompiller
    {
        SreAssemblyBuilder Build(IProject configuration, CompilationMode mode, SqlDatabaseType targetDatabaseType);
    }
}
