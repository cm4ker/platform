#nullable disable

using System;
using System.Collections.Immutable;
using System.Threading;
using Aquila.CodeAnalysis.Semantics;
using Aquila.CodeAnalysis.Syntax;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.PooledObjects;
using Microsoft.CodeAnalysis.Text;

namespace Aquila.CodeAnalysis
{
    public class AquilaSemanticModel : SemanticModel
    {
        private readonly AquilaCompilation _compilation;
        private readonly BinderFactory _binderFactory;
        private readonly SyntaxTree _tree;

        protected override Compilation CompilationCore => _compilation;
        protected override SyntaxTree SyntaxTreeCore => _tree;
        public override bool IsSpeculativeSemanticModel => false;
        public override int OriginalPositionForSpeculation => 0;
        protected override SemanticModel? ParentModelCore => null;
        internal override SemanticModel ContainingModelOrSelf => this;
        protected override SyntaxNode RootCore => _tree.GetRoot();

        public AquilaSemanticModel(AquilaCompilation compilation, SyntaxTree tree)
        {
            _compilation = compilation;
            _binderFactory = _compilation.GetBinderFactory(tree);
            _tree = tree;
        }

        protected override IOperation? GetOperationCore(SyntaxNode node, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        protected override SymbolInfo GetSymbolInfoCore(SyntaxNode node,
            CancellationToken cancellationToken = new CancellationToken())
        {
            switch (node)
            {
                case null:
                    throw new ArgumentNullException(nameof(node));
                case ExprSyntax expression:
                    return this.GetSymbolInfo(expression, cancellationToken);
                // case ConstructorInitializerSyntax initializer:
                //     return this.GetSymbolInfo(initializer, cancellationToken);
                // case PrimaryConstructorBaseTypeSyntax initializer:
                //     return this.GetSymbolInfo(initializer, cancellationToken);
                // case AttributeSyntax attribute:
                //     return this.GetSymbolInfo(attribute, cancellationToken);
                // case CrefSyntax cref:
                //     return this.GetSymbolInfo(cref, cancellationToken);
                // case SelectOrGroupClauseSyntax selectOrGroupClause:
                //     return this.GetSymbolInfo(selectOrGroupClause, cancellationToken);
                // case OrderingSyntax orderingSyntax:
                //     return this.GetSymbolInfo(orderingSyntax, cancellationToken);
                // case PositionalPatternClauseSyntax ppcSyntax:
                //     return this.GetSymbolInfo(ppcSyntax, cancellationToken);
            }

            return SymbolInfo.None;
        }

        protected override SymbolInfo GetSpeculativeSymbolInfoCore(int position, SyntaxNode expression,
            SpeculativeBindingOption bindingOption)
        {
            throw new NotImplementedException();
        }

        protected override TypeInfo GetSpeculativeTypeInfoCore(int position, SyntaxNode expression,
            SpeculativeBindingOption bindingOption)
        {
            throw new NotImplementedException();
        }

        protected override TypeInfo GetTypeInfoCore(SyntaxNode node,
            CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        protected override IAliasSymbol? GetAliasInfoCore(SyntaxNode nameSyntax,
            CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        protected override IAliasSymbol? GetSpeculativeAliasInfoCore(int position, SyntaxNode nameSyntax,
            SpeculativeBindingOption bindingOption)
        {
            throw new NotImplementedException();
        }

        public override ImmutableArray<Diagnostic> GetSyntaxDiagnostics(TextSpan? span = null,
            CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        public override ImmutableArray<Diagnostic> GetDeclarationDiagnostics(TextSpan? span = null,
            CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        public override ImmutableArray<Diagnostic> GetMethodBodyDiagnostics(TextSpan? span = null,
            CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        public override ImmutableArray<Diagnostic> GetDiagnostics(TextSpan? span = null,
            CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        protected override ISymbol? GetDeclaredSymbolCore(SyntaxNode declaration,
            CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        protected override ImmutableArray<ISymbol> GetDeclaredSymbolsCore(SyntaxNode declaration,
            CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        protected override ImmutableArray<ISymbol> LookupSymbolsCore(int position, INamespaceOrTypeSymbol? container,
            string? name,
            bool includeReducedExtensionMethods)
        {
            throw new NotImplementedException();
        }

        protected override ImmutableArray<ISymbol> LookupBaseMembersCore(int position, string? name)
        {
            throw new NotImplementedException();
        }

        protected override ImmutableArray<ISymbol> LookupStaticMembersCore(int position,
            INamespaceOrTypeSymbol? container, string? name)
        {
            throw new NotImplementedException();
        }

        protected override ImmutableArray<ISymbol> LookupNamespacesAndTypesCore(int position,
            INamespaceOrTypeSymbol? container, string? name)
        {
            throw new NotImplementedException();
        }

        protected override ImmutableArray<ISymbol> LookupLabelsCore(int position, string? name)
        {
            throw new NotImplementedException();
        }

        protected override ControlFlowAnalysis AnalyzeControlFlowCore(SyntaxNode firstStatement,
            SyntaxNode lastStatement)
        {
            throw new NotImplementedException();
        }

        protected override ControlFlowAnalysis AnalyzeControlFlowCore(SyntaxNode statement)
        {
            throw new NotImplementedException();
        }

        protected override DataFlowAnalysis AnalyzeDataFlowCore(SyntaxNode firstStatement, SyntaxNode lastStatement)
        {
            throw new NotImplementedException();
        }

        protected override DataFlowAnalysis AnalyzeDataFlowCore(SyntaxNode statementOrExpression)
        {
            throw new NotImplementedException();
        }

        protected override Optional<object?> GetConstantValueCore(SyntaxNode node,
            CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        protected override ImmutableArray<ISymbol> GetMemberGroupCore(SyntaxNode node,
            CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        protected override ISymbol? GetEnclosingSymbolCore(int position,
            CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        protected override bool IsAccessibleCore(int position, ISymbol symbol)
        {
            throw new NotImplementedException();
        }

        protected override bool IsEventUsableAsFieldCore(int position, IEventSymbol eventSymbol)
        {
            throw new NotImplementedException();
        }

        protected override PreprocessingSymbolInfo GetPreprocessingSymbolInfoCore(SyntaxNode nameSyntax)
        {
            throw new NotImplementedException();
        }

        internal override void ComputeDeclarationsInSpan(TextSpan span, bool getSymbol,
            ArrayBuilder<DeclarationInfo> builder,
            CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        internal override void ComputeDeclarationsInNode(SyntaxNode node, ISymbol associatedSymbol, bool getSymbol,
            ArrayBuilder<DeclarationInfo> builder,
            CancellationToken cancellationToken, int? levelsToCompute = null)
        {
            throw new NotImplementedException();
        }

        public override NullableContext GetNullableContext(int position)
        {
            throw new NotImplementedException();
        }

        public override string Language { get; }

        public SymbolInfo GetSymbolInfo(ExprSyntax expression,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            //TODO: separate the Bound nodes and Operations
            //bound nodes need to result ISymbol type because
            //bounding between the SyntaxNode and Symbol
            //return SymbolInfo.None;

            var binder = _binderFactory.GetBinder(expression);
            var type = binder.BindExpression(expression, BoundAccess.None);
            return new SymbolInfo(type.ResultType);
        }
    }
}