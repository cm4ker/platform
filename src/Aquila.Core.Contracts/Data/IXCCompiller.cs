using Aquila.Compiler.Contracts;
using Aquila.Compiler.Roslyn.RoslynBackend;
using Aquila.QueryBuilder;

namespace Aquila.Configuration.Contracts.Data
{
    public interface IXCCompiller
    {
        RoslynAssemblyBuilder Build(IProject configuration, CompilationMode mode, SqlDatabaseType targetDatabaseType);
    }
}
