using ZenPlatform.Compiler;
using ZenPlatform.Compiler.Contracts;
using ZenPlatform.Compiler.Roslyn.RoslynBackend;
using ZenPlatform.QueryBuilder;

namespace ZenPlatform.Configuration.Contracts.Data
{
    public interface IXCCompiller
    {
        RoslynAssemblyBuilder Build(IProject configuration, CompilationMode mode, SqlDatabaseType targetDatabaseType);
    }
}
