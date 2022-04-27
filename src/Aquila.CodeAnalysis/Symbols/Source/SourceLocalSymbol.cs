using System;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Aquila.CodeAnalysis;
using Aquila.CodeAnalysis.Semantics;
using Aquila.CodeAnalysis.Syntax;
using Aquila.Syntax.Ast;
using Microsoft.CodeAnalysis.Symbols;

namespace Aquila.CodeAnalysis.Symbols
{
    /// <summary>
    /// Variable kind.
    /// </summary>
    public enum VariableKind
    {
        /// <summary>
        /// Variable is local in the method.
        /// </summary>
        LocalVariable,

        /// <summary>
        /// Variable is reference to a global variable.
        /// </summary>
        GlobalVariable,

        /// <summary>
        /// Variable refers to a method parameter.
        /// </summary>
        Parameter,

        /// <summary>
        /// Variable is <c>$this</c> variable.
        /// </summary>
        ThisParameter,

        /// <summary>
        /// Variable was introduced with <c>static</c> declaration.
        /// </summary>
        StaticVariable,

        /// <summary>
        /// Variable is a local synthesized variable, must be indirect.
        /// </summary>
        LocalTemporalVariable,
    }

    internal class SourceLocalSymbol : LocalSymbol, ILocalSymbol, ILocalSymbolInternal
    {
        readonly protected SourceMethodSymbol _method;
        readonly protected VariableDecl _decl;

        readonly string _name;

        public SourceLocalSymbol(SourceMethodSymbol method, VariableInit declarator)
        {
            Debug.Assert(method != null);
            Debug.Assert(!string.IsNullOrWhiteSpace(declarator.Identifier.Text));

            _method = method;
            _decl = (VariableDecl)declarator.Parent;
            _name = declarator.Identifier.Text;
        }

        #region Symbol

        public override string Name => _name;

        public override void Accept(SymbolVisitor visitor)
            => visitor.VisitLocal(this);

        public override TResult Accept<TResult>(SymbolVisitor<TResult> visitor)
            => visitor.VisitLocal(this);

        internal override SyntaxNode GetDeclaratorSyntax() => null;
        public override TypeWithAnnotations TypeWithAnnotations { get; }

        public override Symbol ContainingSymbol => _method;


        public override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences
        {
            get { throw new NotImplementedException(); }
        }

        public override ImmutableArray<Location> Locations
        {
            get { throw new NotImplementedException(); }
        }

        #endregion

        #region ILocalSymbol

        public object ConstantValue => null;
        internal override bool IsCompilerGenerated { get; }

        internal override ConstantValue GetConstantValue(SyntaxNode node, LocalSymbol inProgress,
            BindingDiagnosticBag diagnostics = null)
        {
            throw new NotImplementedException();
        }

        internal override ImmutableBindingDiagnostic<AssemblySymbol> GetConstantValueDiagnostics(
            BoundExpression boundInitValue)
        {
            throw new NotImplementedException();
        }

        public bool HasConstantValue => false;

        public bool IsConst => false;

        public bool IsFunctionValue => false;

        internal override SyntaxToken IdentifierToken { get; }

        public override TypeSymbol Type
        {
            get
            {
                var langElem = _decl.Type;

                var binder = DeclaringCompilation.GetBinder(langElem);

                TypeSymbol tsymbol;

                if (langElem.IsVar)
                {
                    tsymbol = (TypeSymbol)binder
                        .BindExpression(_decl.Variables.First().Initializer.Value, BoundAccess.ReadAndWrite)
                        .Type;
                }
                else

                {
                    tsymbol = binder.BindType(langElem);
                }

                //Debug.Assert(tsymbol.IsValidType());
                return tsymbol;
            }
        }

        internal override bool IsPinned { get; }

        internal override LocalSymbol WithSynthesizedLocalKindAndSyntax(SynthesizedLocalKind kind, SyntaxNode syntax)
        {
            throw new NotImplementedException();
        }

        internal override bool IsImportedFromMetadata => false;

        internal override LocalDeclarationKind DeclarationKind { get; }
        internal override SynthesizedLocalKind SynthesizedKind => SynthesizedLocalKind.UserDefined;
        internal override SyntaxNode ScopeDesignatorOpt { get; }

        public virtual bool IsRef => false;

        public override RefKind RefKind => RefKind.None;
        internal override uint RefEscapeScope { get; }
        internal override uint ValEscapeScope { get; }

        public virtual bool IsFixed => false;

        ITypeSymbol ILocalSymbol.Type => Type;

        Microsoft.CodeAnalysis.NullableAnnotation ILocalSymbol.NullableAnnotation =>
            Microsoft.CodeAnalysis.NullableAnnotation.None;

        #endregion
    }
}