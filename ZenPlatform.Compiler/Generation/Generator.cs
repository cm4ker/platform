using System;
using System.Collections.Generic;
using MoreLinq.Extensions;
using ZenPlatform.Compiler.Contracts;
using ZenPlatform.Configuration.Contracts;
using ZenPlatform.Configuration.Structure;
using ZenPlatform.Language.Ast;
using ZenPlatform.Language.Ast.Definitions;
using ZenPlatform.Language.Ast.Definitions.Statements;
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

        private readonly IProject _conf;

        private Root _root;
        private CompilationUnitList _cus;

        private ServerAssemblyServiceScope _serviceScope;

        private SystemTypeBindings _bindings;
        private SyntaxTreeMemberAccessProvider _map;

        //private const string DEFAULT_ASM_NAMESPACE = "CompileNamespace";

        public Generator(GeneratorParameters parameters)
        {
            _parameters = parameters;

            if (_root != null)
            {
                _root = parameters.Root;
                _cus = parameters.Root.Units;
            }

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

        private void CreateBindings()
        {
            var b = new ClassTable();
            b.FillStandard(_bindings);
        }
    }
}