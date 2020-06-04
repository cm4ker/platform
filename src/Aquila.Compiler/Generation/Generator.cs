using System;
using Aquila.Compiler.Contracts;
using Aquila.Core.Contracts;
using Aquila.Core.Contracts.TypeSystem;
using Aquila.Language.Ast.Definitions;
using Aquila.UI.Ast;
using SreTA = System.Reflection.TypeAttributes;

namespace Aquila.Compiler.Generation
{
    public partial class Generator
    {
        private readonly GeneratorParameters _parameters;

        private readonly CompilationMode _mode;
        private readonly IProject _conf;

        private Root _root;
        private CompilationUnitList _cus;

        private EntryPointAssemblyManager _epManager;

        private SystemTypeBindings _bindings;
        private SyntaxTreeMemberAccessProvider _map;

        public Generator(GeneratorParameters parameters)
        {
            _parameters = parameters;

            if (parameters.Root != null)
            {
                _root = parameters.Root;
                _cus = parameters.Root.Units;
            }

            _conf = parameters.Configuration;
            _mode = parameters.Mode;
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