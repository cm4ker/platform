using System;
using Aquila.CodeAnalysis.CodeGen;
using Aquila.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeGen;

namespace Aquila.CodeAnalysis.Symbols.Synthesized
{
    internal partial class SynthesizedMethodSymbol
    {
        public Func<PEModuleBuilder, DiagnosticBag, Action<ILBuilder>> MethodBuilder { get; protected set; }

        public SynthesizedMethodSymbol SetMethodBuilder(Func<PEModuleBuilder, DiagnosticBag, Action<ILBuilder>> value)
        {
            MethodBuilder = value;
            return this;
        }

        public MethodBody CreateMethodBody(PEModuleBuilder module, DiagnosticBag diagnostics)
        {
            var ilBuilder = MethodBuilder(module, diagnostics);
            return MethodGenerator.GenerateMethodBody(module, this, ilBuilder, null, diagnostics, false);
        }
    }
}