using Aquila.Compiler.Roslyn.RoslynBackend;
using Aquila.Core.Contracts.Configuration;
using Aquila.QueryBuilder;

namespace Aquila.WebServiceComponent.Compilation
{
    internal interface IWsInternalGenTask
    {
        RoslynTypeBuilder Stage0(RoslynAssemblyBuilder asm);
        void Stage1(RoslynTypeBuilder builder, SqlDatabaseType dbType, IEntryPointManager sm);
        void Stage2(RoslynTypeBuilder builder, SqlDatabaseType dbType);
    }
}