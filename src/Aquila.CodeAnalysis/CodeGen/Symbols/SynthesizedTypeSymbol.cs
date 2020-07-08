using Aquila.CodeAnalysis.CodeGen;
using Aquila.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis;

namespace Aquila.CodeAnalysis.Symbols.Synthesized
{
    internal partial class SynthesizedTypeSymbol
    {
        public void Init(PEModuleBuilder module, DiagnosticBag diagnostics)
        {
            //emit only SynthesizedMethods SourceMethods iterate in other cycle 
            var methods = GetMembers().OfType<SynthesizedMethodSymbol>();

            foreach (var method in methods)
            {
                module.SetMethodBody(method, method.CreateMethodBody(module, diagnostics));
            }
        }
    }
}