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

    public class SourceOrdinaryMethodSymbol : SourceMethodSymbolWithAttributes
    {
        internal SourceOrdinaryMethodSymbol(string name, ImmutableArray<ParameterSymbol> parameters,
            NamedTypeSymbol namedType,
            MethodDeclarationSyntax? declaration = null)
        {
            // Parameters = parameters;
            // NamedType = namedType;
            // Declaration = declaration;
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