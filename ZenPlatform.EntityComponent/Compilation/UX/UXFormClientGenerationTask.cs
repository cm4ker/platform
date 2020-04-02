using ZenPlatform.Avalonia.Wrapper;
using ZenPlatform.ClientRuntime;
using ZenPlatform.Compiler.Contracts;
using ZenPlatform.Compiler.Helpers;
using ZenPlatform.Configuration.Contracts;
using ZenPlatform.Configuration.Contracts.TypeSystem;
using ZenPlatform.EntityComponent.Configuration;
using ZenPlatform.Language.Ast;
using ZenPlatform.Language.Ast.Definitions;
using ZenPlatform.QueryBuilder;

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

        public ITypeBuilder Stage0(IAssemblyBuilder asm)
        {
            _ts = asm.TypeSystem;
            _sb = _ts.GetSystemBindings();

            var result =
                asm.DefineInstanceType(GetNamespace(), $"{ObjectType.Name}{Name}Form", _ts.FindType<UXForm>());

            return result;
        }

        private ITypeSystem _ts;
        private SystemTypeBindings _sb;
        private IField _markup;
        private IField _params;
        private IField _f_viewModel;
        private ITypeBuilder _viewModel;

        public void Stage1(ITypeBuilder builder, SqlDatabaseType dbType, IEntryPointManager sm)
        {
            builder.DefineDefaultConstructor(false);

            //make entry point registration

            var uxClient = _ts.FindType<UXClient>();
            var gScope = _ts.FindType(typeof(GlobalScope));

            var openWindow = uxClient.FindMethod(nameof(UXClient.OpenWindow), _sb.String, _sb.Object);
            var registerCommand = gScope.FindMethod(nameof(GlobalScope.AddCommand), _sb.String, _sb.Action);
            var g = sm.Main.Generator;

            var m = sm.EntryPoint.DefineMethod("testUI", true, true, false);

            g.LdStr("test")
                .LdNull()
                .LdFtn(m)
                .NewObj(_sb.Action.FindConstructor(_sb.Object, _sb.IntPrt))
                .EmitCall(registerCommand);

            m.Generator
                .RemoteCall(_sb.String, $"UX.{Name}", e => { e.LdNull(); })
                .LdNull()
                .EmitCall(openWindow)
                .Ret();
        }

        public void Stage2(ITypeBuilder builder, SqlDatabaseType dbType)
        {
        }
    }
}