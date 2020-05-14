using Aquila.Compiler;
using Aquila.Compiler.Contracts;
using Aquila.Compiler.Roslyn;
using Aquila.Compiler.Roslyn.RoslynBackend;
using Aquila.Core.Contracts;
using Aquila.Core.Contracts.Configuration;
using Aquila.Core.Contracts.TypeSystem;
using Aquila.EntityComponent.Configuration;
using Aquila.Language.Ast;
using Aquila.Language.Ast.Definitions;
using Aquila.Language.Ast.Symbols;
using Aquila.QueryBuilder;

namespace Aquila.EntityComponent.Compilation
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