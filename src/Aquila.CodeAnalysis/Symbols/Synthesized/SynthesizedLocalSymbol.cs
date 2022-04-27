using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Symbols;

namespace Aquila.CodeAnalysis.Symbols
{
    internal class SynthesizedLocalSymbol : LocalSymbol, ILocalSymbol, ILocalSymbolInternal
    {
        private readonly MethodSymbol _method;
        private readonly string _name;
        readonly TypeSymbol _type;
        private Microsoft.CodeAnalysis.NullableAnnotation _nullableAnnotation;

        public SynthesizedLocalSymbol(MethodSymbol method, string name, TypeSymbol type)
        {
            Contract.ThrowIfNull(type);
            _method = method;
            _name = name;
            _type = type;
        }

        public override string Name => _name;


        public override Symbol ContainingSymbol => _method;
        public override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences { get; }

        public override TypeSymbol Type { get; }
        internal override bool IsPinned { get; }

        internal override SyntaxToken IdentifierToken { get; }


        ITypeSymbol ILocalSymbol.Type => Type;

        Microsoft.CodeAnalysis.NullableAnnotation ILocalSymbol.NullableAnnotation => _nullableAnnotation;

        public Microsoft.CodeAnalysis.NullableAnnotation NullableAnnotation =>
            Microsoft.CodeAnalysis.NullableAnnotation.None;

        public bool IsConst => false;

        internal override ConstantValue GetConstantValue(SyntaxNode node, LocalSymbol inProgress,
            BindingDiagnosticBag diagnostics = null)
        {
            throw new System.NotImplementedException();
        }

        internal override ImmutableBindingDiagnostic<AssemblySymbol> GetConstantValueDiagnostics(
            BoundExpression boundInitValue)
        {
            throw new System.NotImplementedException();
        }

        public bool IsRef { get; }
        public override RefKind RefKind { get; }
        internal override uint RefEscapeScope { get; }
        internal override uint ValEscapeScope { get; }
        public bool HasConstantValue { get; }
        public object? ConstantValue { get; }
        internal override bool IsCompilerGenerated { get; }
        public bool IsFunctionValue { get; }
        public bool IsFixed => false;

        internal override LocalSymbol WithSynthesizedLocalKindAndSyntax(SynthesizedLocalKind kind, SyntaxNode syntax)
        {
            throw new System.NotImplementedException();
        }

        internal override SyntaxNode GetDeclaratorSyntax()
        {
            return null;
        }

        public override TypeWithAnnotations TypeWithAnnotations { get; }

        internal override bool IsImportedFromMetadata { get; }

        internal override LocalDeclarationKind DeclarationKind { get; }
        internal override SynthesizedLocalKind SynthesizedKind => SynthesizedLocalKind.LoweringTemp;
        internal override SyntaxNode ScopeDesignatorOpt { get; }
    }
}