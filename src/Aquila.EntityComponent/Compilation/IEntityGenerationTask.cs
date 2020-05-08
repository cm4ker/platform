using Aquila.Compiler.Roslyn.RoslynBackend;
using Aquila.Configuration.Contracts;
using Aquila.QueryBuilder;

namespace Aquila.EntityComponent.Compilation
{
    public interface IEntityGenerationTask
    {
        RoslynTypeBuilder Stage0(RoslynAssemblyBuilder asm);
        void Stage1(RoslynTypeBuilder builder, SqlDatabaseType dbType, IEntryPointManager sm);
        void Stage2(RoslynTypeBuilder builder, SqlDatabaseType dbType);
    }
}