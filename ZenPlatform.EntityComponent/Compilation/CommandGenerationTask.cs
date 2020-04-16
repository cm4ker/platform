using ZenPlatform.Compiler;
using ZenPlatform.Compiler.Contracts;
using ZenPlatform.Compiler.Roslyn;
using ZenPlatform.Compiler.Roslyn.DnlibBackend;
using ZenPlatform.Configuration.Contracts;
using ZenPlatform.Configuration.Contracts.TypeSystem;
using ZenPlatform.EntityComponent.Configuration;
using ZenPlatform.Language.Ast;
using ZenPlatform.Language.Ast.Definitions;
using ZenPlatform.Language.Ast.Symbols;
using ZenPlatform.QueryBuilder;

namespace ZenPlatform.EntityComponent.Compilation
{
    public class CommandGenerationTask : ComponentAstTask, IEntityGenerationTask
    {
        public CommandGenerationTask(MDCommand command, CompilationMode compilationMode, IComponent component,
            string name) : base(compilationMode, component, true, name, TypeBody.Empty)
        {
            Command = command;

            Init();
        }

        public MDCommand Command { get; }

        private void Init()
        {
            var typeBody = ParserHelper.ParseTypeBody(Command.Module.ModuleText);

            foreach (var func in typeBody.Functions)
            {
                func.SymbolScope = SymbolScopeBySecurity.User;
            }

            Replace(TypeBody, typeBody);
        }

        public RoslynTypeBuilder Stage0(RoslynAssemblyBuilder asm)
        {
            return asm.DefineStaticType(GetNamespace(), Name);
        }

        public void Stage1(RoslynTypeBuilder builder, SqlDatabaseType dbType, IEntryPointManager sm)
        {
        }

        public void Stage2(RoslynTypeBuilder builder, SqlDatabaseType dbType)
        {
        }
    }
}