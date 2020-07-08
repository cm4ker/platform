using System.Linq;
using Portable.Xaml;
using Aquila.Avalonia.Wrapper;
using Aquila.Compiler.Contracts;
//using Aquila.Compiler.Contracts;
using Aquila.Compiler.Generation;
using Aquila.Compiler.Helpers;
using Aquila.Compiler.Roslyn;
using Aquila.Compiler.Roslyn.RoslynBackend;
using Aquila.Core.Contracts;
using Aquila.Core.Contracts.Configuration;
using Aquila.Core.Contracts.TypeSystem;
using Aquila.EntityComponent.Configuration;
using Aquila.Language.Ast;
using Aquila.Language.Ast.Definitions;
using Aquila.QueryBuilder;
using Aquila.ServerRuntime;
using Name = Aquila.Language.Ast.Name;
using SystemTypeBindings = Aquila.Compiler.Roslyn.SystemTypeBindings;

namespace Aquila.EntityComponent.Compilation.UX
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

        public RoslynTypeBuilder Stage0(RoslynAssemblyBuilder asm)
        {
            _ts = asm.TypeSystem;
            _sb = _ts.GetSystemBindings();

            var result =
                asm.DefineStaticType(GetNamespace(), $"__{ObjectType.Name}{Name}Form");


            return result;
        }

        private RoslynTypeSystem _ts;
        private SystemTypeBindings _sb;
        private RoslynMethodBuilder _getMethod;
        private RoslynField _markup;
        private RoslynMethod _xamlServiceParse;
        private RoslynMethod _xamlServiceSave;


        public void Stage1(RoslynTypeBuilder builder, SqlDatabaseType dbType, IEntryPointManager sm)
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
                .LdLoc(loc)
                .Call(formType.FindMethod(nameof(UXForm.CreateOnServer)))
                .LdLoc(loc)
                .Call(_xamlServiceSave)
                .Ret();

            EmitRegisterServerFunction(sm);
        }


        private void EmitRegisterServerFunction(IEntryPointManager sm)
        {
            var e = sm.Main.Body;
            var invs = sm.GetISField();

            var dlgt = sm.EntryPoint.DefineMethod($"dlgt_{Name}", true, true, false);

            dlgt.DefineParameter("context", _ts.InvokeContext(), false, false);
            var argsParam = dlgt.DefineParameter("args", _sb.Object.MakeArrayType(), false, false);
            dlgt.WithReturnType(_sb.Object);

            var dle = dlgt.Body;

            dle.Call(_getMethod);

            dle.Ret();

            e.LdSFld(invs)
                .LdLit($"UX.{Name}")
                .NewObj(_ts.Route().Constructors.First())
                //.Null()
                .LdFtn(dlgt)
                //.NewObj(_sb.ParametricMethod.Constructors.First())
                .Call(_ts.InvokeService().FindMethod(m => m.Name == "Register"))
                ;
        }


        public void Stage2(RoslynTypeBuilder builder, SqlDatabaseType dbType)
        {
        }
    }
}