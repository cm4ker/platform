using System.Collections.Immutable;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.PooledObjects;
using Microsoft.CodeAnalysis.Text;

namespace Aquila.CodeAnalysis
{
    public class SyntaxTreeSemanticModel : AquilaSemanticModel
    {
        public SyntaxTreeSemanticModel()
        {
        }

        protected override IOperation? GetOperationCore(SyntaxNode node, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        protected override SymbolInfo GetSymbolInfoCore(SyntaxNode node,
            CancellationToken cancellationToken = new CancellationToken())
        {
            throw new System.NotImplementedException();
        }

        protected override SymbolInfo GetSpeculativeSymbolInfoCore(int position, SyntaxNode expression,
            SpeculativeBindingOption bindingOption)
        {
            throw new System.NotImplementedException();
        }

        protected override TypeInfo GetSpeculativeTypeInfoCore(int position, SyntaxNode expression,
            SpeculativeBindingOption bindingOption)
        {
            throw new System.NotImplementedException();
        }

        protected override TypeInfo GetTypeInfoCore(SyntaxNode node,
            CancellationToken cancellationToken = new CancellationToken())
        {
            throw new System.NotImplementedException();
        }

        protected override IAliasSymbol? GetAliasInfoCore(SyntaxNode nameSyntax,
            CancellationToken cancellationToken = new CancellationToken())
        {
            throw new System.NotImplementedException();
        }

        protected override IAliasSymbol? GetSpeculativeAliasInfoCore(int position, SyntaxNode nameSyntax,
            SpeculativeBindingOption bindingOption)
        {
            throw new System.NotImplementedException();
        }

        public override ImmutableArray<Diagnostic> GetSyntaxDiagnostics(TextSpan? span = null,
            CancellationToken cancellationToken = new CancellationToken())
        {
            throw new System.NotImplementedException();
        }

        public override ImmutableArray<Diagnostic> GetDeclarationDiagnostics(TextSpan? span = null,
            CancellationToken cancellationToken = new CancellationToken())
        {
            throw new System.NotImplementedException();
        }

        public override ImmutableArray<Diagnostic> GetMethodBodyDiagnostics(TextSpan? span = null,
            CancellationToken cancellationToken = new CancellationToken())
        {
            throw new System.NotImplementedException();
        }

        public override ImmutableArray<Diagnostic> GetDiagnostics(TextSpan? span = null,
            CancellationToken cancellationToken = new CancellationToken())
        {
            throw new System.NotImplementedException();
        }

        protected override ISymbol? GetDeclaredSymbolCore(SyntaxNode declaration,
            CancellationToken cancellationToken = new CancellationToken())
        {
            throw new System.NotImplementedException();
        }

        protected override ImmutableArray<ISymbol> GetDeclaredSymbolsCore(SyntaxNode declaration,
            CancellationToken cancellationToken = new CancellationToken())
        {
            throw new System.NotImplementedException();
        }

        protected override ImmutableArray<ISymbol> LookupSymbolsCore(int position, INamespaceOrTypeSymbol? container,
            string? name,
            bool includeReducedExtensionMethods)
        {
            throw new System.NotImplementedException();
        }

        protected override ImmutableArray<ISymbol> LookupBaseMembersCore(int position, string? name)
        {
            throw new System.NotImplementedException();
        }

        protected override ImmutableArray<ISymbol> LookupStaticMembersCore(int position,
            INamespaceOrTypeSymbol? container, string? name)
        {
            throw new System.NotImplementedException();
        }

        protected override ImmutableArray<ISymbol> LookupNamespacesAndTypesCore(int position,
            INamespaceOrTypeSymbol? container, string? name)
        {
            throw new System.NotImplementedException();
        }

        protected override ImmutableArray<ISymbol> LookupLabelsCore(int position, string? name)
        {
            throw new System.NotImplementedException();
        }

        protected override ControlFlowAnalysis AnalyzeControlFlowCore(SyntaxNode firstStatement,
            SyntaxNode lastStatement)
        {
            throw new System.NotImplementedException();
        }

        protected override ControlFlowAnalysis AnalyzeControlFlowCore(SyntaxNode statement)
        {
            throw new System.NotImplementedException();
        }

        protected override DataFlowAnalysis AnalyzeDataFlowCore(SyntaxNode firstStatement, SyntaxNode lastStatement)
        {
            throw new System.NotImplementedException();
        }

        protected override DataFlowAnalysis AnalyzeDataFlowCore(SyntaxNode statementOrExpression)
        {
            throw new System.NotImplementedException();
        }

        protected override Optional<object?> GetConstantValueCore(SyntaxNode node,
            CancellationToken cancellationToken = new CancellationToken())
        {
            throw new System.NotImplementedException();
        }

        protected override ImmutableArray<ISymbol> GetMemberGroupCore(SyntaxNode node,
            CancellationToken cancellationToken = new CancellationToken())
        {
            throw new System.NotImplementedException();
        }

        protected override ISymbol? GetEnclosingSymbolCore(int position,
            CancellationToken cancellationToken = new CancellationToken())
        {
            throw new System.NotImplementedException();
        }

        protected override bool IsAccessibleCore(int position, ISymbol symbol)
        {
            throw new System.NotImplementedException();
        }

        protected override bool IsEventUsableAsFieldCore(int position, IEventSymbol eventSymbol)
        {
            throw new System.NotImplementedException();
        }

        protected override PreprocessingSymbolInfo GetPreprocessingSymbolInfoCore(SyntaxNode nameSyntax)
        {
            throw new System.NotImplementedException();
        }

        internal override void ComputeDeclarationsInSpan(TextSpan span, bool getSymbol,
            ArrayBuilder<DeclarationInfo> builder,
            CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        internal override void ComputeDeclarationsInNode(SyntaxNode node, ISymbol associatedSymbol, bool getSymbol,
            ArrayBuilder<DeclarationInfo> builder,
            CancellationToken cancellationToken, int? levelsToCompute = null)
        {
            throw new System.NotImplementedException();
        }

        public override NullableContext GetNullableContext(int position)
        {
            throw new System.NotImplementedException();
        }

        public override string Language => "Aquila";
        protected override Compilation CompilationCore { get; }
        protected override SyntaxTree SyntaxTreeCore { get; }
        public override bool IsSpeculativeSemanticModel { get; }
        public override int OriginalPositionForSpeculation { get; }
        protected override SemanticModel? ParentModelCore { get; }
        internal override SemanticModel ContainingModelOrSelf { get; }
        protected override SyntaxNode RootCore { get; }
    }
}