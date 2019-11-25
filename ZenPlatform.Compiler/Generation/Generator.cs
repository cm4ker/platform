using System;
using System.Collections.Generic;
using ZenPlatform.Compiler.Contracts;
using ZenPlatform.Configuration.Contracts;
using ZenPlatform.Configuration.Structure;
using ZenPlatform.Language.Ast.Definitions;
using ZenPlatform.UI.Ast;
using SreTA = System.Reflection.TypeAttributes;

namespace ZenPlatform.Compiler.Generation
{
    public partial class Generator
    {
        private readonly GeneratorParameters _parameters;

        //private readonly CompilationUnit _cu;
        private readonly IAssemblyBuilder _asm;
        private readonly ITypeSystem _ts;
        private readonly CompilationMode _mode;

        private readonly IXCRoot _conf;


        private List<CompilationUnit> _cus;

        private ServerAssemblyServiceScope _serviceScope;

        private SystemTypeBindings _bindings;

        private const string DEFAULT_ASM_NAMESPACE = "CompileNamespace";

        public Generator(GeneratorParameters parameters)
        {
            _parameters = parameters;
            _cus = parameters.Units;
            _asm = parameters.Builder;
            _ts = _asm.TypeSystem;

            _conf = parameters.Configuration;

            _mode = parameters.Mode;

            _bindings = _ts.GetSystemBindings();
        }

        private void Error(string message)
        {
            throw new Exception(message);
        }

        private void EmitUI(UINode node)
        {
        }
    }
}