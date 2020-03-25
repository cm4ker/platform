using System.Collections.Generic;
using dnlib.DotNet.Resources;
using NLog.Targets.Wrappers;
using ReactiveUI;
using ZenPlatform.Compiler.Contracts;
using ZenPlatform.Configuration.Contracts.TypeSystem;
using ZenPlatform.EntityComponent.Configuration;
using ZenPlatform.Language.Ast;
using ZenPlatform.Language.Ast.Definitions;
using ZenPlatform.QueryBuilder;

namespace ZenPlatform.EntityComponent.Compilation.UX
{
    /*
        Form
        {
            Dic Parameters
            SetParameter(string, Object);
            ViewModel Model
                        
            string Markup;
            
            Show()
        } 
      
     */


    public class FormGenerationTask : ComponentAstTask, IEntityGenerationTask
    {
        private readonly MDInterface _md;

        public FormGenerationTask(
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

            _viewModel = asm.DefineInstanceType(GetNamespace(), ObjectType.Name + Name + "FormViewModel");
            _viewModel.DefineDefaultConstructor(false);

            return asm.DefineStaticType(GetNamespace(), ObjectType.Name + Name + "Form");
        }

        private ITypeSystem _ts;
        private SystemTypeBindings _sb;
        private IField _markup;
        private IField _params;
        private IField _f_viewModel;
        private ITypeBuilder _viewModel;

        public void Stage1(ITypeBuilder builder, SqlDatabaseType dbType)
        {
            _markup = builder.DefineField(_sb.String, "_markup", false, false);

            var dctType = _ts.FindType(typeof(Dictionary<,>)).MakeGenericType(_sb.String, _sb.Object);
            _params = builder.DefineField(dctType, "_params", false, false);

            _f_viewModel = builder.DefineField(_viewModel, "_vm", false, false);

            var c = builder.DefineConstructor(false);
            c.Generator
                .LdArg_0()
                .LdStr(_md.Markup)
                .StFld(_markup)
                .LdArg_0()
                .NewObj(dctType.FindConstructor())
                .StFld(_params)
                .LdArg_0()
                .NewObj(_viewModel.FindConstructor())
                .StFld(_f_viewModel)
                .LdArg_0()
                .EmitCall(builder.BaseType.Constructors[0])
                .Ret();


            var m = builder.DefineMethod("Show", true, true, false);

            var g = m.Generator;

            g.Ret();
        }

        public void Stage2(ITypeBuilder builder, SqlDatabaseType dbType)
        {
        }
    }
}