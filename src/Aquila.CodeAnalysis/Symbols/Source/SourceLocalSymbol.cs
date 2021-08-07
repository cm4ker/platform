using System;
using System.Collections.Immutable;
using System.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Aquila.CodeAnalysis;
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

    internal class SourceLocalSymbol : Symbol, ILocalSymbol, ILocalSymbolInternal
    {
        readonly protected SourceMethodSymbol _method;

        readonly string _name;

        public SourceLocalSymbol(SourceMethodSymbol method, string name, TextSpan span)
        {
            Debug.Assert(method != null);
            Debug.Assert(!string.IsNullOrWhiteSpace(name));

            _method = method;
            _name = name;
        }

        #region Symbol

        public override string Name => _name;

        public override void Accept(SymbolVisitor visitor)
            => visitor.VisitLocal(this);

        public override TResult Accept<TResult>(SymbolVisitor<TResult> visitor)
            => visitor.VisitLocal(this);

        public SyntaxNode GetDeclaratorSyntax() => null;

        public override Symbol ContainingSymbol => _method;

        public override Accessibility DeclaredAccessibility => Accessibility.Private;

        public override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences
        {
            get { throw new NotImplementedException(); }
        }

        public override bool IsAbstract => false;

        public override bool IsExtern => false;

        public override bool IsOverride => false;

        public override bool IsSealed => true;

        public override bool IsStatic => false;

        public override bool IsVirtual => false;

        public override SymbolKind Kind => SymbolKind.Local;

        public override ImmutableArray<Location> Locations
        {
            get { throw new NotImplementedException(); }
        }

        internal override ObsoleteAttributeData ObsoleteAttributeData => null;

        #endregion

        #region ILocalSymbol

        public object ConstantValue => null;

        public bool HasConstantValue => false;

        public bool IsConst => false;

        public bool IsFunctionValue => false;

        public virtual ITypeSymbol Type
        {
            get
            {
                var type = _method.ControlFlowGraph.GetLocalType(this.Name);
                var langElem = _method.SyntaxReturnType;
                var binder = DeclaringCompilation.GetBinder(langElem);
                var tsymbol = binder.BindType(langElem);

                Debug.Assert(tsymbol.IsValidType());
                return tsymbol;
            }
        }

        public bool IsImportedFromMetadata => false;

        public SynthesizedLocalKind SynthesizedKind => SynthesizedLocalKind.UserDefined;

        public virtual bool IsRef => false;

        public virtual RefKind RefKind => RefKind.None;

        public virtual bool IsFixed => false;

        NullableAnnotation ILocalSymbol.NullableAnnotation => NullableAnnotation.None;

        #endregion
    }

    internal class SynthesizedLocalSymbol : SourceLocalSymbol
    {
        readonly TypeSymbol _type;

        public SynthesizedLocalSymbol(SourceMethodSymbol method, string name, TypeSymbol type)
            : base(method, name + "'", default(TextSpan))
        {
            Contract.ThrowIfNull(type);
            _type = type;
        }

        public override ITypeSymbol Type => _type;
    }
}