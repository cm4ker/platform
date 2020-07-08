using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeGen;
using Aquila.CodeAnalysis.Symbols;
using System;
using System.Diagnostics;
using System.Reflection.Metadata;
using Aquila.CodeAnalysis.CodeGen;
using Aquila.CodeAnalysis.Symbols.Synthesized;

namespace Aquila.CodeAnalysis.Emit
{
    partial class PEModuleBuilder
    {
        /// <summary>
        /// Create real CIL entry point, where it calls given method.
        /// </summary>
        internal void CreateEntryPoint(DiagnosticBag diagnostic)
        {
            // "static int Main(string[] args)"
            var realmethod = new SynthesizedMethodSymbol(this.EntryPointType, "Main", true, false,
                _compilation.CoreTypes.Int32, Accessibility.Private);

            realmethod.SetParameters(new SynthesizedParameterSymbol(realmethod,
                ArrayTypeSymbol.CreateSZArray(this.Compilation.SourceAssembly, this.Compilation.CoreTypes.Object), 0,
                RefKind.None, "args"));

            //
            var body = MethodGenerator.GenerateMethodBody(this, realmethod,
                (il) =>
                {
                    il.EmitIntConstant(0);
                    il.EmitRet(false);
                },
                null, diagnostic, false);

            SetMethodBody(realmethod, body);

            //
            this.EntryPointType.EntryPointSymbol = realmethod;
        }

        /// <summary>
        /// Emits body of scripts main wrapper.
        /// </summary>
        /// <param name="wrapper">&lt;Main&gt;`0 method, that calls real Main.</param>
        /// <param name="main">Real scripts main method.</param>
        /// <param name="diagnostic">DiagnosticBag.</param>
        internal void CreateMainMethodWrapper(MethodSymbol wrapper, MethodSymbol main, DiagnosticBag diagnostic)
        {
            if (wrapper == null)
                return;

            // generate body of <wrapper> calling <main>
            Debug.Assert(wrapper.IsStatic);
            Debug.Assert(main.IsStatic);

            Debug.Assert(wrapper.ParameterCount == main.ParameterCount);

            //
            var body = MethodGenerator.GenerateMethodBody(this, wrapper,
                (il) => { },
                null, diagnostic, false);

            SetMethodBody(wrapper, body);
        }
    }
}