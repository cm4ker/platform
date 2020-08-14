using Microsoft.CodeAnalysis;
using Pchp.CodeAnalysis.Emit;

namespace Aquila.CodeAnalysis.Symbols
{
    partial class SourceFileSymbol
    {
        internal void SynthesizeInit(PEModuleBuilder module, DiagnosticBag diagnostics)
        {
            // module.EmitBootstrap(this); // unnecessary
        }
    }
}