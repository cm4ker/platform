using Cci = Microsoft.Cci;

namespace Aquila.CodeAnalysis.Symbols
{
    internal partial class NamespaceSymbol : Cci.INamespace
    {
        Cci.INamespace Cci.INamespace.ContainingNamespace => this.ContainingNamespace as Cci.INamespace;
        string Cci.INamedEntity.Name => MetadataName;
    }
}
