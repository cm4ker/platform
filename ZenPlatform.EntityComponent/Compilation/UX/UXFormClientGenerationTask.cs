using ZenPlatform.Avalonia.Wrapper;
using ZenPlatform.ClientRuntime;
using ZenPlatform.Compiler.Contracts;
//using ZenPlatform.Compiler.Contracts;
using ZenPlatform.Compiler.Helpers;
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
    public class UXFormClientGenerationTask : ComponentAstTask, IEntityGenerationTask
    {
        private readonly MDInterface _md;

        public UXFormClientGenerationTask(
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
            _sb = _ts.GetSystemBindings();

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
            builder.DefineDefaultConstructor(false);

            //make entry point registration

            var uxClient = _ts.FindType<UXClient>();
            var gScope = _ts.FindType(typeof(GlobalScope));

            var openWindow = uxClient.FindMethod(nameof(UXClient.OpenWindow), _sb.String, _sb.Object);
            var registerCommand = gScope.FindMethod(nameof(GlobalScope.AddCommand), _sb.String, _sb.Action);
            var g = sm.Main.Body;

            var m = sm.EntryPoint.DefineMethod("testUI", true, true, false);

            g.LdLit("test")
                //.LdNull()
                .LdFtn(m)
                //.NewObj(_sb.Action.FindConstructor(_sb.Object, _sb.IntPrt))
                .Call(registerCommand);

            m.Body
                .RemoteCall(_sb.String, $"UX.{Name}", e => { e.LdNull(); })
                .Null()
                .Call(openWindow)
                .Ret()
                .Statement();
        }

        public void Stage2(SreTypeBuilder builder, SqlDatabaseType dbType)
        {
        }
    }
}