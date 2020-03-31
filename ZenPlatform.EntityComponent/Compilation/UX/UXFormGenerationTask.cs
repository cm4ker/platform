using System.Collections.Generic;
using System.Linq;
using dnlib.DotNet.Resources;
using NLog.Targets.Wrappers;
using Portable.Xaml;
using ReactiveUI;
using ZenPlatform.Avalonia.Wrapper;
using ZenPlatform.Compiler.Contracts;
using ZenPlatform.Configuration.Contracts;
using ZenPlatform.Configuration.Contracts.TypeSystem;
using ZenPlatform.EntityComponent.Configuration;
using ZenPlatform.Language.Ast;
using ZenPlatform.Language.Ast.Definitions;
using ZenPlatform.QueryBuilder;

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

        public ITypeBuilder Stage0(IAssemblyBuilder asm)
        {
            _ts = asm.TypeSystem;
            _sb = _ts.GetSystemBindings();

            // _viewModel = asm.DefineInstanceType(GetNamespace(), ObjectType.Name + Name + "FormViewModel");
            // _viewModel.DefineDefaultConstructor(false);

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

        public void Stage1(ITypeBuilder builder, SqlDatabaseType dbType, IAssemblyServiceManager sm)
        {
            builder.DefineDefaultConstructor(false);
        }

        public void Stage2(ITypeBuilder builder, SqlDatabaseType dbType)
        {
        }
    }

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

        public ITypeBuilder Stage0(IAssemblyBuilder asm)
        {
            _ts = asm.TypeSystem;
            _sb = _ts.GetSystemBindings();

            var result =
                asm.DefineStaticType(GetNamespace(), $"__{ObjectType.Name}{Name}Form");


            return result;
        }

        private ITypeSystem _ts;
        private SystemTypeBindings _sb;
        private IMethodBuilder _getMethod;
        private IField _markup;
        private IMethod _xamlServiceParse;


        public void Stage1(ITypeBuilder builder, SqlDatabaseType dbType, IAssemblyServiceManager sm)
        {
            var formType = builder.Assembly.FindType($"{GetNamespace()}.{ObjectType.Name}{Name}Form");

            _getMethod = builder.DefineMethod("Get", true, true, false);
            _getMethod.WithReturnType(formType);

            _xamlServiceParse = _ts.FindType(typeof(XamlServices)).FindMethod(nameof(XamlServices.Parse), _sb.String);

            var g = _getMethod.Generator;

            var loc = g.DefineLocal(formType);


            _markup = builder.DefineField(_sb.String, "_markup", false, true);

            var c = builder.DefineConstructor(true);
            c.Generator
                .LdStr(_md.Markup)
                .StSFld(_markup)
                .Ret();

            g.LdSFld(_markup)
                .EmitCall(_xamlServiceParse)
                .CastClass(formType)
                .StLoc(loc)
                .LdLoc(loc)
                .EmitCall(formType.FindMethod(nameof(UXForm.CreateOnServer)))
                .LdLoc(loc)
                .Ret();

            EmitRegisterServerFunction(sm);
        }


        private void EmitRegisterServerFunction(IAssemblyServiceManager sm)
        {
            var e = sm.ServiceInitializerInitMethod.Generator;
            var invs = sm.InvokeServiceField;

            var dlgt = sm.ServiceInitializerType.DefineMethod($"dlgt_{Name}", true, false, false);

            dlgt.DefineParameter("context", _sb.InvokeContext, false, false);
            var argsParam = dlgt.DefineParameter("args", _sb.Object.MakeArrayType(), false, false);
            dlgt.WithReturnType(_sb.Object);

            var dle = dlgt.Generator;

            // for (int i = 0; i < method.Parameters.Count; i++)
            // {
            //     dle.LdArg(argsParam)
            //         .LdcI4(i)
            //         .LdElemRef()
            //         .Unbox_Any(method.Parameters[i].Type);
            // }

            dle.EmitCall(_getMethod);


            dle.Box(_getMethod.ReturnType).Ret();

            e.LdArg_0()
                .LdFld(invs)
                .LdStr($"UX.{Name}")
                .NewObj(_sb.Route.Constructors.First())
                .LdArg_0()
                .LdFtn(dlgt)
                .NewObj(_sb.ParametricMethod.Constructors.First())
                .EmitCall(_sb.InvokeService.FindMethod((m) => m.Name == "Register"));
        }


        public void Stage2(ITypeBuilder builder, SqlDatabaseType dbType)
        {
        }
    }
}