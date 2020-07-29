using System.Collections.Immutable;
using Aquila.Language.Ast.Definitions.Functions;

namespace Aquila.Language.Ast.Symbols
{
    public class MethodSymbol : Symbol
    {
        public override SymbolKind Kind => SymbolKind.Method;
        public MethodDeclarationSyntax? Declaration { get; }
        public ImmutableArray<ParameterSymbol> Parameters { get; }
        public NamedTypeSymbol NamedType { get; }
    }


    public abstract class SourceMethodSymbol : MethodSymbol
    {
    }

    public abstract class SourceMethodSymbolWithAttributes : SourceMethodSymbol
    {
    }

    public class SourceOrdinaryMethodSymbol : SourceMemberMethodSymbol
    {
        internal SourceOrdinaryMethodSymbol(NamedTypeSymbol containingType, MethodDeclarationSyntax syntax) : base(
            containingType)
        {
        }
    }

    public abstract class SourceMemberMethodSymbol : SourceMethodSymbol
    {
        private readonly NamedTypeSymbol _containingType;

        protected SourceMemberMethodSymbol(NamedTypeSymbol containingType)
        {
            _containingType = containingType;
        }
    }
}