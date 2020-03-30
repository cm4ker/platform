using System.Collections.Generic;
using dnlib.DotNet.Resources;
using NLog.Targets.Wrappers;
using Portable.Xaml;
using ReactiveUI;
using ZenPlatform.Avalonia.Wrapper;
using ZenPlatform.Compiler.Contracts;
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

        public void Stage1(ITypeBuilder builder, SqlDatabaseType dbType)
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


        public void Stage1(ITypeBuilder builder, SqlDatabaseType dbType)
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
        }

        public void Stage2(ITypeBuilder builder, SqlDatabaseType dbType)
        {
        }
    }
}