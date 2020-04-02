using ZenPlatform.Compiler.Contracts;
using ZenPlatform.Configuration.Contracts;
using ZenPlatform.Configuration.Contracts.TypeSystem;
using ZenPlatform.EntityComponent.Entity;
using ZenPlatform.Language.Ast;
using ZenPlatform.Language.Ast.Definitions;
using ZenPlatform.QueryBuilder;

namespace ZenPlatform.EntityComponent.Compilation
{
    public class DtoTableRowGenerationTask : ComponentAstTask, IEntityGenerationTask
    {
        public DtoTableRowGenerationTask(
            IPType dtoType,
            ITable table,
            CompilationMode compilationMode, IComponent component, string name, TypeBody tb)
            : base(compilationMode, component, false, name, tb)
        {
            DtoType = dtoType;
            Table = table;
        }

        public IPType DtoType { get; }
        public ITable Table { get; }

        public ITypeBuilder Stage0(IAssemblyBuilder asm)
        {
            var type = asm.DefineInstanceType(this.GetNamespace(), Table.GetDtoRowClassName());

            type.DefineDefaultConstructor(false);

            return type;
        }

        public void Stage1(ITypeBuilder builder, SqlDatabaseType dbType, IEntryPointManager sm)
        {
            EmitBody(builder, dbType);
        }

        public void Stage2(ITypeBuilder builder, SqlDatabaseType dbType)
        {
        }

        private void EmitBody(ITypeBuilder builder, SqlDatabaseType dbType)
        {
            var ts = builder.Assembly.TypeSystem;
            var sb = ts.GetSystemBindings();
            var tm = Table.TypeManager;

            var ownerType = tm.FindType(Table.ParentId);
            var @namespace = ownerType.GetNamespace();

            var dtoType = ts.FindType(ownerType.Name);

            foreach (var prop in Table.Properties)
            {
                SharedGenerators.EmitDtoProperty(builder, prop, sb);
            }
        }
    }
}