using System.Linq;
using Portable.Xaml;
using ZenPlatform.Avalonia.Wrapper;
using ZenPlatform.Compiler.Contracts;
//using ZenPlatform.Compiler.Contracts;
using ZenPlatform.Compiler.Generation;
using ZenPlatform.Compiler.Roslyn;
using ZenPlatform.Compiler.Roslyn.DnlibBackend;
using ZenPlatform.Configuration.Contracts;
using ZenPlatform.Configuration.Contracts.TypeSystem;
using ZenPlatform.EntityComponent.Configuration;
using ZenPlatform.Language.Ast;
using ZenPlatform.Language.Ast.Definitions;
using ZenPlatform.QueryBuilder;
using ZenPlatform.ServerRuntime;
using SystemTypeBindings = ZenPlatform.Compiler.Roslyn.SystemTypeBindings;

namespace ZenPlatform.EntityComponent.Compilation.UX
{
    public class FormStaticActionsGenerationTask : ComponentAstTask, IEntityGenerationTask
    {
        private readonly MDInterface _md;

        public FormStaticActionsGenerationTask(
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
                asm.DefineStaticType(GetNamespace(), $"__{ObjectType.Name}{Name}Form");


            return result;
        }

        private SreTypeSystem _ts;
        private SystemTypeBindings _sb;
        private SreMethodBuilder _getMethod;
        private SreField _markup;
        private SreMethod _xamlServiceParse;
        private SreMethod _xamlServiceSave;


        public void Stage1(SreTypeBuilder builder, SqlDatabaseType dbType, IEntryPointManager sm)
        {
            var formType = builder.Assembly.FindType($"{GetNamespace()}.{ObjectType.Name}{Name}Form");

            _getMethod = builder.DefineMethod("Get", true, true, false);
            _getMethod.WithReturnType(_sb.String);

            _xamlServiceParse = _ts.ResolveType(typeof(XamlService)).FindMethod(nameof(XamlServices.Parse), _sb.String);
            _xamlServiceSave = _ts.ResolveType(typeof(XamlService)).FindMethod(nameof(XamlServices.Save), _sb.Object);
            var g = _getMethod.Body;

            var loc = g.DefineLocal(formType);


            _markup = builder.DefineField(_sb.String, "_markup", false, true);

            var c = builder.DefineConstructor(true);
            c.Body
                .LdLit(_md.Markup)
                .StSFld(_markup)
                .Ret();

            g.LdSFld(_markup)
                .Call(_xamlServiceParse)
                .Cast(formType)
                .StLoc(loc)
                .Statement()
                .LdLoc(loc)
                .Call(formType.FindMethod(nameof(UXForm.CreateOnServer)))
                .Statement()
                .LdLoc(loc)
                .Call(_xamlServiceSave)
                .Ret().Statement();

            EmitRegisterServerFunction(sm);
        }


        private void EmitRegisterServerFunction(IEntryPointManager sm)
        {
            var e = sm.Main.Body;
            var invs = sm.GetISField();

            var dlgt = sm.EntryPoint.DefineMethod($"dlgt_{Name}", true, true, false);

            dlgt.DefineParameter("context", _sb.InvokeContext, false, false);
            var argsParam = dlgt.DefineParameter("args", _sb.Object.MakeArrayType(), false, false);
            dlgt.WithReturnType(_sb.Object);

            var dle = dlgt.Body;

            dle.Call(_getMethod);

            dle.Ret().Statement();

            e.LdSFld(invs)
                .LdLit($"UX.{Name}")
                .NewObj(_sb.Route.Constructors.First())
                //.Null()
                .LdFtn(dlgt)
                //.NewObj(_sb.ParametricMethod.Constructors.First())
                .Call(_sb.InvokeService.FindMethod(m => m.Name == "Register"))
                .Statement();
        }


        public void Stage2(SreTypeBuilder builder, SqlDatabaseType dbType)
        {
        }
    }
}