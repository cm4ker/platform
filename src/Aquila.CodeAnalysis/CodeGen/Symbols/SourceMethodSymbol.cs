using Microsoft.CodeAnalysis;
using Aquila.CodeAnalysis.CodeGen;
using System;
using System.Collections.Generic;
using Aquila.CodeAnalysis.Emit;

namespace Aquila.CodeAnalysis.Symbols
{
    partial class SourceMethodSymbol
    {
        /// <summary>
        /// Creates ghost stubs,
        /// i.e. methods with a different signature calling this method to comply with CLR standards.
        /// </summary>
        /// <returns>List of additional overloads.</returns>
        internal virtual IList<MethodSymbol> SynthesizeStubs(PEModuleBuilder module, DiagnosticBag diagnostic)
        {
            //
            EmitParametersDefaultValue(module, diagnostic);

            // TODO: resolve this already in SourceTypeSymbol.GetMembers(), now it does not get overloaded properly
            return SynthesizeOverloadsWithOptionalParameters(module, diagnostic);
        }

        /// <summary>
        /// Synthesizes method overloads in case there are optional parameters which explicit default value cannot be resolved as a <see cref="ConstantValue"/>.
        /// </summary>
        /// <remarks>
        /// foo($a = [], $b = [1, 2, 3])
        /// + foo() => foo([], [1, 2, 3)
        /// + foo($a) => foo($a, [1, 2, 3])
        /// </remarks>
        protected IList<MethodSymbol> SynthesizeOverloadsWithOptionalParameters(PEModuleBuilder module,
            DiagnosticBag diagnostic)
        {
            List<MethodSymbol> list = null;

            var implparams = ImplicitParameters;
            var srcparams = SourceParameters;
            var implicitVarArgs = VarargsParam;

            for (int i = 0; i <= srcparams.Length; i++) // how many to be copied from {srcparams}
            {
                var isfake = /*srcparams[i - 1].IsFake*/ implicitVarArgs != null && i > 0 &&
                                                         srcparams[i - 1].Ordinal >=
                                                         implicitVarArgs
                                                             .Ordinal; // parameter was replaced with [params]
                var
                    hasdefault =
                        false; // i < srcparams.Length && srcparams[i].HasUnmappedDefaultValue();  // ConstantValue couldn't be resolved for optional parameter
            }

            return list ?? (IList<MethodSymbol>)Array.Empty<MethodSymbol>();
        }

        private void EmitParametersDefaultValue(PEModuleBuilder module, DiagnosticBag diagnostics)
        {
        }

        public virtual void Generate(CodeGenerator cg)
        {
            cg.GenerateScope(this.ControlFlowGraph.Start, int.MaxValue);
        }

        private void GenerateGeneratorMethod(CodeGenerator cg)
        {
        }
    }

    partial class SourceGeneratorSymbol
    {
        internal void EmitInit(PEModuleBuilder module)
        {
            // Don't need any initial emit
        }
    }
};