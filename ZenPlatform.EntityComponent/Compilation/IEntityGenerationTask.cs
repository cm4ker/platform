using ZenPlatform.Compiler.Roslyn.DnlibBackend;
using ZenPlatform.Configuration.Contracts;
using ZenPlatform.QueryBuilder;

namespace ZenPlatform.EntityComponent.Compilation
{
    public interface IEntityGenerationTask
    {
        RoslynTypeBuilder Stage0(RoslynAssemblyBuilder asm);
        void Stage1(RoslynTypeBuilder builder, SqlDatabaseType dbType, IEntryPointManager sm);
        void Stage2(RoslynTypeBuilder builder, SqlDatabaseType dbType);
    }
}