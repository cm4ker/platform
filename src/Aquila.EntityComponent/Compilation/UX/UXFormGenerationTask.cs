using Aquila.Avalonia.Wrapper;
using Aquila.Compiler.Contracts;
using Aquila.Compiler.Roslyn;
using Aquila.Compiler.Roslyn.RoslynBackend;
using Aquila.Core.Contracts;
using Aquila.Core.Contracts.Configuration;
using Aquila.Core.Contracts.TypeSystem;
using Aquila.EntityComponent.Configuration;
using Aquila.Language.Ast;
using Aquila.QueryBuilder;
using Name = Aquila.Language.Ast.Name;
using SystemTypeBindings = Aquila.Compiler.Roslyn.SystemTypeBindings;

namespace Aquila.EntityComponent.Compilation.UX
{
    /*
     1) From (UXForm) <---- Here client code inside form
     2) Static class for invoker (client -> server)
     3) ViewModel for form
     */

    public class UXFormGenerationTask : ComponentAstTask, IEntityGenerationTask
    {
        private readonly MDInterface _md;

        public UXFormGenerationTask(
            IPType objectType, MDInterface md, CompilationMode compilationMode, IComponent component, bool isModule,
            string name,
            TypeBody tb) : base(compilationMode, component, isModule, name, tb)
        {
            _md = md;
            ObjectType = objectType;
        }

        public MDInterface MD { get; }

        public IPType ObjectType { get; }

        public RoslynTypeBuilder Stage0(RoslynAssemblyBuilder asm)
        {
            _ts = asm.TypeSystem;

            // _viewModel = asm.DefineInstanceType(GetNamespace(), ObjectType.Name + Name + "FormViewModel");
            // _viewModel.DefineDefaultConstructor(false);

            var result =
                asm.DefineInstanceType(GetNamespace(), $"{ObjectType.Name}{Name}Form", _ts.Resolve<UXForm>());

            return result;
        }

        private RoslynTypeSystem _ts;
        private SystemTypeBindings _sb;
        private RoslynField _markup;
        private RoslynField _params;
        private RoslynField _f_viewModel;
        private RoslynTypeBuilder _viewModel;

        public void Stage1(RoslynTypeBuilder builder, SqlDatabaseType dbType, IEntryPointManager sm)
        {
            // builder.DefineDefaultConstructor(false);
        }

        public void Stage2(RoslynTypeBuilder builder, SqlDatabaseType dbType)
        {
        }
    }
}