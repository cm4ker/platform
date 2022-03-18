using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Symbols;

namespace Aquila.CodeAnalysis.Symbols
{
    internal class SynthesizedLocalSymbol : Symbol, ILocalSymbol, ILocalSymbolInternal
    {
        private readonly MethodSymbol _method;
        private readonly string _name;
        readonly TypeSymbol _type;

        public SynthesizedLocalSymbol(MethodSymbol method, string name, TypeSymbol type)
        {
            Contract.ThrowIfNull(type);
            _method = method;
            _name = name;
            _type = type;
        }

        public override string Name => _name;

        internal override ObsoleteAttributeData ObsoleteAttributeData { get; }
        public override SymbolKind Kind => SymbolKind.Local;
        public override Symbol ContainingSymbol => _method;
        public override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences { get; }
        public override Accessibility DeclaredAccessibility => Accessibility.Private;
        public override bool IsStatic => false;
        public override bool IsVirtual => false;
        public override bool IsOverride => false;
        public override bool IsAbstract => false;
        public override bool IsSealed => false;
        public override bool IsExtern => false;
        public ITypeSymbol Type => _type;
        public NullableAnnotation NullableAnnotation => NullableAnnotation.None;
        public bool IsConst => false;
        public bool IsRef { get; }
        public RefKind RefKind => RefKind.None;
        public bool HasConstantValue { get; }
        public object? ConstantValue { get; }
        public bool IsFunctionValue { get; }
        public bool IsFixed => false;

        public SyntaxNode GetDeclaratorSyntax() => null;
        public bool IsImportedFromMetadata => false;
        public SynthesizedLocalKind SynthesizedKind => SynthesizedLocalKind.LoweringTemp;
    }
}