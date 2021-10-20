using Microsoft.CodeAnalysis.Symbols;
using Cci = Microsoft.Cci;

namespace Aquila.CodeAnalysis.Symbols
{
    internal partial class NamespaceSymbol : Cci.INamespace
    {
        public INamespaceSymbolInternal GetInternalSymbol()
        {
            throw new System.NotImplementedException();
        }

        Cci.INamespace Cci.INamespace.ContainingNamespace => this.ContainingNamespace as Cci.INamespace;
        string Cci.INamedEntity.Name => MetadataName;
    }
}
