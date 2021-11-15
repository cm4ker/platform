using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading;
using Aquila.Syntax.Ast;
using Aquila.Syntax.Ast.Functions;
using Aquila.Syntax.Ast.Statements;
using Microsoft.CodeAnalysis;
using Aquila.CodeAnalysis.FlowAnalysis;
using Aquila.CodeAnalysis.Semantics;
using Aquila.CodeAnalysis.Semantics.Graph;
using Aquila.CodeAnalysis.Symbols.Attributes;
using Aquila.CodeAnalysis.Symbols.Source;
using Aquila.CodeAnalysis.Symbols.Synthesized;
using Aquila.CodeAnalysis.Syntax;
using Aquila.Syntax;
using Aquila.Syntax.Text;
using Microsoft.CodeAnalysis.Text;

namespace Aquila.CodeAnalysis.Symbols
{
    /// <summary>
    /// Represents a Aquila class method.
    /// </summary>
    internal partial class SourceMethodSymbol : MethodSymbol
    {
        readonly NamedTypeSymbol _type;

        readonly MethodDecl _syntax;

        TextSpan NameSpan => _syntax.Identifier.Span;

        MethodSymbol _lazyOverridenMethod;

        public SourceMethodSymbol(NamedTypeSymbol type, MethodDecl syntax)
        {
            Contract.ThrowIfNull(type);
            Contract.ThrowIfNull(syntax);

            _type = type;
            _syntax = syntax;
        }

        internal bool RequiresLateStaticBoundParam =>
            IsStatic && // `static` in instance method == typeof($this)
            ControlFlowGraph != null && // cfg sets {Flags}
            (this.Flags & MethodFlags.UsesLateStatic) != 0
        //&& (!_type.IsSealed || _type.IsTrait)
        ; // `static` == `self` <=> self is sealed

        public override IMethodSymbol OverriddenMethod
        {
            get
            {
                if ((_commonflags & CommonFlags.OverriddenMethodResolved) == 0)
                {
                    Interlocked.CompareExchange(ref _lazyOverridenMethod, this.ResolveOverride(), null);

                    Debug.Assert(_lazyOverridenMethod != this);

                    _commonflags |= CommonFlags.OverriddenMethodResolved;
                }

                return _lazyOverridenMethod;
            }
        }

        internal ParameterListSyntax SyntaxSignature => _syntax.ParameterList;

        internal TypeEx SyntaxReturnType => _syntax.ReturnType;

        internal AquilaSyntaxNode Syntax => _syntax;

        internal IList<StmtSyntax> Statements => _syntax.Body?.Statements.ToList();

        protected TypeRefContext CreateTypeRefContext() => TypeRefFactory.CreateTypeRefContext(_type);

        public void GetDiagnostics(DiagnosticBag diagnostic)
        {
            var name = _syntax.Identifier.Text;
        }

        public override string Name => _syntax.Identifier.Text;

        public override Symbol ContainingSymbol => _type;

        public override Accessibility DeclaredAccessibility
        {
            get
            {
                var accessibility = _syntax.GetModifiers().GetAccessibility();

                if (_syntax.IsGlobal)
                    accessibility = Accessibility.Public;

                return accessibility;
            }
        }

        public override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences
        {
            get { throw new NotImplementedException(); }
        }

        public override TypeSymbol ReturnType
        {
            get { return DeclaringCompilation.GetBinder(_syntax).BindType(SyntaxReturnType); }
        }

        public override bool IsStatic => _syntax.GetModifiers().IsStatic();

        public override bool IsAbstract => false; //!IsStatic && (_syntax.Modifiers.IsAbstract() || _type.IsInterface);

        public override bool IsOverride => IsVirtual && this.OverriddenMethod != null &&
                                           this.SignaturesMatch((MethodSymbol)this.OverriddenMethod);

        public override bool IsSealed => false; //_syntax.Modifiers.IsSealed() && IsVirtual;

        public override bool IsVirtual
        {
            get
            {
                if (IsStatic || DeclaredAccessibility == Accessibility.Private)
                {
                    return false;
                }

                return false;
            }
        }

        [Flags]
        protected enum CommonFlags
        {
            OverriddenMethodResolved = 1,
        }

        /// <summary>Internal true/false values. Initially all false.</summary>
        protected CommonFlags _commonflags;

        ControlFlowGraph _cfg;
        LocalsTable _locals;

        /// <summary>
        /// Lazily bound semantic block.
        /// Entry point of analysis and emitting.
        /// </summary>
        public override ControlFlowGraph ControlFlowGraph
        {
            get
            {
                if (_cfg == null && this.Statements != null) // ~ Statements => non abstract method
                {
                    // create initial flow state
                    var state = StateBinder.CreateInitialState(this);

                    var binder = DeclaringCompilation.GetBinder(_syntax);

                    // build control flow graph
                    var cfg = new ControlFlowGraph(this.Statements, binder);
                    cfg.Start.FlowState = state;

                    //
                    Interlocked.CompareExchange(ref _cfg, cfg, null);
                }

                //
                return _cfg;
            }
            internal set { _cfg = value; }
        }

        /// <summary>
        /// Gets table of local variables.
        /// Variables are lazily added to the table.
        /// </summary>
        internal LocalsTable LocalsTable
        {
            get
            {
                if (_locals == null)
                {
                    Interlocked.CompareExchange(ref _locals, new LocalsTable(this), null);
                }

                return _locals;
            }
        }

        public override ImmutableArray<Location> Locations =>
            ImmutableArray.Create(
                Location.Create(null, Syntax is LangElement element ? element.Span.ToTextSpan() : default
                ));

        public override bool IsUnreachable => (Flags & MethodFlags.IsUnreachable) != 0;

        protected ImmutableArray<ParameterSymbol> _implicitParameters;
        private SourceParameterSymbol[] _srcParams;

        /// <summary>Implicitly declared [params] parameter if the method allows access to its arguments. This allows more arguments to be passed than declared.</summary>
        private SynthesizedParameterSymbol _implicitVarArg; // behaves like a stack of optional parameters

        /// <summary>
        /// Builds implicit parameters before source parameters.
        /// </summary>
        /// <returns></returns>
        protected virtual IEnumerable<ParameterSymbol> BuildImplicitParams()
        {
            var index = 0;

            if (IsStatic)
            {
                // Context <ctx>
                yield return new SpecialParameterSymbol(this, DeclaringCompilation.CoreTypes.AqContext,
                    SpecialParameterSymbol.ContextName, index++);
            }

            yield break;
        }


        /// <summary>
        /// Constructs method source parameters.
        /// </summary>
        protected IEnumerable<SourceParameterSymbol> BuildSrcParams(
            IEnumerable<Parameter> formalparams)
        {
            var pindex = 0; // zero-based relative index

            foreach (var p in formalparams)
            {
                if (p == null)
                {
                    continue;
                }

                yield return new SourceParameterSymbol(this, p, relindex: pindex++ /*, ptagOpt: ptag*/);
            }
        }

        internal virtual ImmutableArray<ParameterSymbol> ImplicitParameters
        {
            get
            {
                if (_implicitParameters.IsDefault)
                {
                    ImmutableInterlocked.InterlockedInitialize(ref _implicitParameters,
                        BuildImplicitParams().ToImmutableArray());
                }

                var currentImplicitParameters = _implicitParameters;
                if (RequiresLateStaticBoundParam &&
                    !currentImplicitParameters.Any(SpecialParameterSymbol.IsLateStaticParameter))
                {
                    var implicitParameters = currentImplicitParameters.Add(
                        new SpecialParameterSymbol(this, null,
                            SpecialParameterSymbol.StaticTypeName, currentImplicitParameters.Length));
                    ImmutableInterlocked.InterlockedCompareExchange(ref _implicitParameters, implicitParameters,
                        currentImplicitParameters);
                }

                //
                return _implicitParameters;
            }
        }

        internal SourceParameterSymbol[] SourceParameters
        {
            get
            {
                if (_srcParams == null)
                {
                    var srcParams = BuildSrcParams(this.SyntaxSignature).ToArray();
                    Interlocked.CompareExchange(ref _srcParams, srcParams, null);
                }

                return _srcParams;
            }
        }

        SourceParameterSymbol SourceVarargsParam
        {
            get
            {
                var srcparams = this.SourceParameters;
                if (srcparams.Length != 0)
                {
                    var last = srcparams.Last();
                    if (last.IsParams)
                    {
                        return last;
                    }
                }

                return null;
            }
        }

        /// <summary>
        /// Implicitly added parameter corresponding to Replaces all the optional parameters.
        /// !!IMPORTANT!! Its <see cref="ParameterSymbol.Ordinal"/> specifies its position - all the source parameters with the same or higher ordinal are ignored.
        /// Can be <c>null</c> if not needed.
        /// </summary>
        protected ParameterSymbol VarargsParam
        {
            get
            {
                // declare implicit [... varargs] parameter if needed and not defined as source parameter
                if ((Flags & MethodFlags.RequiresVarArg) != 0 && !IsGlobalScope)
                {
                    if (_implicitVarArg == null)
                    {
                        var srcparams = SourceVarargsParam;

                        // is there is params (...) already and no optional parameters, we can stick with it
                        if (srcparams != null && SourceParameters.All(p => p.Initializer == null))
                        {
                            return null;
                        }

                        // create implicit [... params]
                        var implicitVarArg = new SynthesizedParameterSymbol( // IsImplicitlyDeclared, IsParams
                            this,
                            ArrayTypeSymbol.CreateSZArray(this.ContainingAssembly, null),
                            0,
                            RefKind.None,
                            SpecialParameterSymbol.ParamsName, isParams: true);
                        Interlocked.CompareExchange(ref _implicitVarArg, implicitVarArg, null);
                    }
                }

                if (_implicitVarArg != null)
                {
                    // implicit params replaces all the optional arguments!!
                    int mandatory = ImplicitParameters.Length +
                                    this.SourceParameters.TakeWhile(p => p.Initializer == null).Count();
                    _implicitVarArg.UpdateOrdinal(mandatory);
                }

                return _implicitVarArg;
            }
        }

        /// <summary>
        /// Gets params parameter or null.
        /// </summary>
        internal ParameterSymbol GetParamsParameter()
        {
            var p = VarargsParam ?? SourceVarargsParam;
            Debug.Assert(p == null || p.Type.IsSZArray());

            return p;
        }

        public override bool IsExtern => false;


        public override bool CastToFalse => false; // source methods never cast special values to FALSE

        public override bool HasNotNull => !ReturnsNull;

        public override MethodKind MethodKind
        {
            get
            {
                // TODO: ctor, dtor, props, magic, ...

                return MethodKind.Ordinary;
            }
        }

        public sealed override ImmutableArray<ParameterSymbol> Parameters
        {
            get
            {
                // [implicit parameters], [source parameters], [...varargs]

                var srcparams = SourceParameters;
                var implicitVarArgs = VarargsParam;

                var result = new List<ParameterSymbol>(ImplicitParameters.Length + srcparams.Length);

                result.AddRange(ImplicitParameters);

                if (implicitVarArgs == null)
                {
                    result.AddRange(srcparams);
                }
                else
                {
                    // implicitVarArgs replaces optional srcparams
                    for (int i = 0; i < srcparams.Length && srcparams[i].Ordinal < implicitVarArgs.Ordinal; i++)
                    {
                        result.Add(srcparams[i]);
                    }

                    result.Add(implicitVarArgs);
                }

                return result.AsImmutableOrEmpty();
            }
        }

        public sealed override int ParameterCount
        {
            get
            {
                // [implicit parameters], [source parameters], [...varargs]

                var implicitVarArgs = VarargsParam;
                if (implicitVarArgs != null)
                {
                    return implicitVarArgs.Ordinal + 1;
                }
                else
                {
                    return ImplicitParameters.Length + SourceParameters.Length;
                }
            }
        }

        public override bool ReturnsVoid => ReturnType.SpecialType == SpecialType.System_Void;

        /// <summary>
        /// Gets value indicating the method can return <c>null</c>.
        /// </summary>
        public bool ReturnsNull
        {
            get { return true; }
        }

        public override RefKind RefKind => RefKind.None;

        public override ImmutableArray<AttributeData> GetReturnTypeAttributes()
        {
            if (!ReturnsNull)
            {
                // [return: NotNull]
                var returnType = this.ReturnType;
                if (returnType != null && (returnType.IsReferenceType)
                   ) // only if it makes sense to check for NULL
                {
                    return ImmutableArray.Create<AttributeData>(DeclaringCompilation.CreateNotNullAttribute());
                }
            }

            //
            return ImmutableArray<AttributeData>.Empty;
        }

        internal override ObsoleteAttributeData ObsoleteAttributeData
        {
            get { return null; }
        }

        public override ImmutableArray<AttributeData> GetAttributes()
        {
            var builder = ImmutableArray.CreateBuilder<AttributeData>();
            builder.AddRange(base.GetAttributes());

            foreach (var annotation in _syntax.Annotations)
            {
                var tref = new NamedTypeRef(Span.Empty, SyntaxKind.Type, annotation.Identifier.Text);
                var type = (NamedTypeSymbol)DeclaringCompilation.GetBinder(_syntax).BindType(tref);
                builder.Add(new SourceAttributeData(null, type, type.Ctor(), false));
            }


            return builder.ToImmutable();
        }

        internal override bool IsMetadataNewSlot(bool ignoreInterfaceImplementationChanges = false) =>
            !IsOverride && IsMetadataVirtual(ignoreInterfaceImplementationChanges);

        internal override bool IsMetadataVirtual(bool ignoreInterfaceImplementationChanges = false)
        {
            return IsVirtual &&
                   (!ContainingType.IsSealed || IsOverride || IsAbstract ||
                    OverrideOfMethod() != null); // do not make method virtual if not necessary
        }

        /// <summary>
        /// Gets value indicating the method is an override of another virtual method.
        /// In such a case, this method MUST be virtual.
        /// </summary>
        private MethodSymbol OverrideOfMethod()
        {
            var overrides =
                ContainingType.ResolveOverrides(DiagnosticBag
                    .GetInstance()); // Gets override resolution matrix. This is already resolved and does not cause an overhead.

            for (int i = 0; i < overrides.Length; i++)
            {
                if (overrides[i].Override == this)
                {
                    return overrides[i].Method;
                }
            }

            return null;
        }

        internal override bool IsMetadataFinal =>
            base.IsMetadataFinal &&
            IsMetadataVirtual(); // altered IsMetadataVirtual -> causes change to '.final' metadata as well

        public override string GetDocumentationCommentXml(CultureInfo preferredCulture = null,
            bool expandIncludes = false, CancellationToken cancellationToken = default(CancellationToken))
        {
            return string.Empty;
        }
    }
}