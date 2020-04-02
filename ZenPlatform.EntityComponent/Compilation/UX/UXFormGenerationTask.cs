using ZenPlatform.Avalonia.Wrapper;
using ZenPlatform.Compiler.Contracts;
using ZenPlatform.Compiler.Roslyn;
using ZenPlatform.Compiler.Roslyn.DnlibBackend;
using ZenPlatform.Configuration.Contracts;
using ZenPlatform.Configuration.Contracts.TypeSystem;
using ZenPlatform.EntityComponent.Configuration;
using ZenPlatform.Language.Ast;
using ZenPlatform.Language.Ast.Definitions;
using ZenPlatform.QueryBuilder;
using SystemTypeBindings = ZenPlatform.Compiler.Roslyn.SystemTypeBindings;

namespace ZenPlatform.EntityComponent.Compilation.UX
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

        public SreTypeBuilder Stage0(SreAssemblyBuilder asm)
        {
            _ts = asm.TypeSystem;

            // _viewModel = asm.DefineInstanceType(GetNamespace(), ObjectType.Name + Name + "FormViewModel");
            // _viewModel.DefineDefaultConstructor(false);

            var result =
                asm.DefineInstanceType(GetNamespace(), $"{ObjectType.Name}{Name}Form", _ts.Resolve<UXForm>());

            return result;
        }

        private SreTypeSystem _ts;
        private SystemTypeBindings _sb;
        private SreField _markup;
        private SreField _params;
        private SreField _f_viewModel;
        private SreTypeBuilder _viewModel;

        public void Stage1(SreTypeBuilder builder, SqlDatabaseType dbType, IEntryPointManager sm)
        {
            // builder.DefineDefaultConstructor(false);
        }

        public void Stage2(SreTypeBuilder builder, SqlDatabaseType dbType)
        {
        }
    }
}