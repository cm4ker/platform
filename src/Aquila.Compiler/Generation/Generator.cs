using System;
using System.Collections.Generic;
using MoreLinq.Extensions;
using Aquila.Compiler.Contracts;
using Aquila.Compiler.Roslyn.RoslynBackend;
using Aquila.Configuration.Contracts;
using Aquila.Configuration.Structure;
using Aquila.Language.Ast;
using Aquila.Language.Ast.Definitions;
using Aquila.Language.Ast.Definitions.Statements;
using Aquila.ServerRuntime;
using Aquila.UI.Ast;
using SreTA = System.Reflection.TypeAttributes;
using SystemTypeBindings = Aquila.Compiler.Roslyn.SystemTypeBindings;

namespace Aquila.Compiler.Generation
{
    public partial class Generator
    {
        private readonly GeneratorParameters _parameters;

        //private readonly CompilationUnit _cu;
        private readonly RoslynAssemblyBuilder _asm;
        private readonly RoslynTypeSystem _ts;
        private readonly CompilationMode _mode;

        private readonly IProject _conf;

        private Root _root;
        private CompilationUnitList _cus;

        private EntryPointAssemblyManager _epManager;

        private SystemTypeBindings _bindings;
        private SyntaxTreeMemberAccessProvider _map;

        //private const string DEFAULT_ASM_NAMESPACE = "CompileNamespace";

        public Generator(GeneratorParameters parameters)
        {
            _parameters = parameters;

            if (parameters.Root != null)
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

        // private void CreateBindings()
        // {
        //     var b = new ClassTable();
        //     b.FillStandard(_bindings);
        // }
    }
}