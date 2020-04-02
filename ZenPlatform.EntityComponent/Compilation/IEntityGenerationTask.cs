using ZenPlatform.Compiler.Roslyn.DnlibBackend;
using ZenPlatform.Configuration.Contracts;
using ZenPlatform.QueryBuilder;

namespace ZenPlatform.EntityComponent.Compilation
{
    public interface IEntityGenerationTask
    {
        SreTypeBuilder Stage0(SreAssemblyBuilder asm);
        void Stage1(SreTypeBuilder builder, SqlDatabaseType dbType, IEntryPointManager sm);
        void Stage2(SreTypeBuilder builder, SqlDatabaseType dbType);
    }
}