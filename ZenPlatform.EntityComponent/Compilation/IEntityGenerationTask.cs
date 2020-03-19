using ZenPlatform.Compiler.Contracts;
using ZenPlatform.QueryBuilder;

namespace ZenPlatform.EntityComponent.Compilation
{
    public interface IEntityGenerationTask
    {
        ITypeBuilder Stage0(IAssemblyBuilder asm);
        void Stage1(ITypeBuilder builder, SqlDatabaseType dbType);
        void Stage2(ITypeBuilder builder, SqlDatabaseType dbType);
    }
}