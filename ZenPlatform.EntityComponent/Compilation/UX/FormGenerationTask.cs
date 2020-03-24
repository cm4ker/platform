using ZenPlatform.Compiler.Contracts;
using ZenPlatform.Configuration.Contracts.TypeSystem;
using ZenPlatform.EntityComponent.Configuration;
using ZenPlatform.Language.Ast;
using ZenPlatform.Language.Ast.Definitions;
using ZenPlatform.QueryBuilder;

namespace ZenPlatform.EntityComponent.Compilation.UX
{
    public class FormGenerationTask : ComponentAstTask, IEntityGenerationTask
    {
        public FormGenerationTask(
            IPType objectType, MDInterface md, CompilationMode compilationMode, IComponent component, bool isModule,
            string name,
            TypeBody tb) : base(compilationMode, component, isModule, name, tb)
        {
            ObjectType = objectType;
        }

        public MDInterface MD { get; }

        public IPType ObjectType { get; }

        public ITypeBuilder Stage0(IAssemblyBuilder asm)
        {
            return asm.DefineStaticType(GetNamespace(), ObjectType.Name + Name + "Form");
        }

        public void Stage1(ITypeBuilder builder, SqlDatabaseType dbType)
        {
            var m = builder.DefineMethod("Show", true, true, false);
            m.Generator.Ret();
        }

        public void Stage2(ITypeBuilder builder, SqlDatabaseType dbType)
        {
        }
    }
}