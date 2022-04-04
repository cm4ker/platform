using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Threading;
using Aquila.CodeAnalysis.Errors;
using Microsoft.CodeAnalysis;
using Roslyn.Utilities;

namespace Aquila.CodeAnalysis.Symbols
{
    /// <summary>
    /// An ErrorSymbol is used when the compiler cannot determine a symbol object to return because
    /// of an error. For example, if a field is declared "Foo x;", and the type "Foo" cannot be
    /// found, an ErrorSymbol is returned when asking the field "x" what it's type is.
    /// </summary>
    internal abstract partial class ErrorTypeSymbol : NamedTypeSymbol, IErrorTypeSymbol
    {
        internal static readonly ErrorTypeSymbol UnknownResultType = new UnsupportedMetadataTypeSymbol();

        public abstract CandidateReason CandidateReason { get; }

        public override string Name => string.Empty;

        public override int Arity => 0;

        internal override bool HasTypeArgumentsCustomModifiers => false;

        public override ImmutableArray<CustomModifier> GetTypeArgumentCustomModifiers(int ordinal) =>
            GetEmptyTypeArgumentCustomModifiers(ordinal);

        public override TypeKind TypeKind => TypeKind.Error;

        public override SymbolKind Kind => SymbolKind.ErrorType;

        public override Accessibility DeclaredAccessibility => Accessibility.NotApplicable;

        public virtual ImmutableArray<ISymbol> CandidateSymbols => ImmutableArray<ISymbol>.Empty;

        /// <summary>
        /// Called by <see cref="AbstractTypeMap.SubstituteType(TypeSymbol)"/> to perform substitution
        /// on types with TypeKind ErrorType.  The general pattern is to use the type map
        /// to perform substitution on the wrapped type, if any, and then construct a new
        /// error type symbol from the result (if there was a change).
        /// </summary>
        internal TypeWithAnnotations Substitute(AbstractTypeMap typeMap)
        {
            return TypeWithAnnotations.Create(typeMap.SubstituteNamedType(this));
        }

        internal override bool MangleName => false;

        internal override bool IsWindowsRuntimeImport => false;

        internal override bool ShouldAddWinRTMembers => false;

        internal override TypeLayout Layout => default(TypeLayout);

        public override Symbol ContainingSymbol => null;

        public override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences =>
            ImmutableArray<SyntaxReference>.Empty;

        public override bool IsStatic => false;

        public override bool IsAbstract => false;

        public override bool IsSealed => false;

        public override bool IsSerializable => false;

        internal override bool IsInterface => false;

        internal override ObsoleteAttributeData ObsoleteAttributeData => null;

        internal override IEnumerable<IFieldSymbol> GetFieldsToEmit()
        {
            yield break;
        }

        internal override ImmutableArray<NamedTypeSymbol> GetDeclaredInterfaces(ConsList<Symbol> basesBeingResolved) =>
            ImmutableArray<NamedTypeSymbol>.Empty;

        internal override ImmutableArray<NamedTypeSymbol> GetInterfacesToEmit() =>
            ImmutableArray<NamedTypeSymbol>.Empty;

        public override ImmutableArray<Symbol> GetMembers() => ImmutableArray<Symbol>.Empty;

        public override ImmutableArray<Symbol> GetMembers(string name) => ImmutableArray<Symbol>.Empty;


        public override ImmutableArray<NamedTypeSymbol> GetTypeMembers() => ImmutableArray<NamedTypeSymbol>.Empty;

        public override ImmutableArray<NamedTypeSymbol> GetTypeMembers(string name) =>
            ImmutableArray<NamedTypeSymbol>.Empty;
    }

    internal sealed class UnsupportedMetadataTypeSymbol : ErrorTypeSymbol
    {
        readonly BadImageFormatException _mrEx;

        internal UnsupportedMetadataTypeSymbol(BadImageFormatException mrEx = null)
        {
            _mrEx = mrEx;
        }

        public override CandidateReason CandidateReason => CandidateReason.None;

        internal override bool MangleName
        {
            get { return false; }
        }
    }

    internal class MissingMetadataTypeSymbol : ErrorTypeSymbol
    {
        protected readonly string name;
        protected readonly int arity;
        protected readonly bool mangleName;

        /// <summary>
        /// Represents not nested missing type.
        /// </summary>
        internal sealed class TopLevel : MissingMetadataTypeSymbol
        {
            private readonly string _namespaceName;
            private readonly ModuleSymbol _containingModule;
            private readonly bool _isNativeInt;
            private DiagnosticInfo? _lazyErrorInfo;
            private NamespaceSymbol? _lazyContainingNamespace;

            /// <summary>
            /// Either <see cref="SpecialType"/>, <see cref="WellKnownType"/>, or -1 if not initialized.
            /// </summary>
            private int _lazyTypeId;

            public TopLevel(ModuleSymbol module, string @namespace, string name, int arity, bool mangleName)
                : this(module, @namespace, name, arity, mangleName, errorInfo: null, isNativeInt: false,
                    containingNamespace: null, typeId: -1)
            {
            }

            public TopLevel(ModuleSymbol module, ref MetadataTypeName fullName, DiagnosticInfo? errorInfo = null)
                : this(module, ref fullName, -1, errorInfo)
            {
            }

            public TopLevel(ModuleSymbol module, ref MetadataTypeName fullName, SpecialType specialType,
                DiagnosticInfo? errorInfo = null)
                : this(module, ref fullName, (int)specialType, errorInfo)
            {
            }

            public TopLevel(ModuleSymbol module, ref MetadataTypeName fullName, WellKnownType wellKnownType,
                DiagnosticInfo? errorInfo = null)
                : this(module, ref fullName, (int)wellKnownType, errorInfo)
            {
            }

            private TopLevel(ModuleSymbol module, ref MetadataTypeName fullName, int typeId, DiagnosticInfo? errorInfo)
                : this(module, ref fullName,
                    fullName.ForcedArity == -1 || fullName.ForcedArity == fullName.InferredArity, errorInfo, typeId)
            {
            }

            private TopLevel(ModuleSymbol module, ref MetadataTypeName fullName, bool mangleName,
                DiagnosticInfo? errorInfo, int typeId)
                : this(module, fullName.NamespaceName,
                    mangleName ? fullName.UnmangledTypeName : fullName.TypeName,
                    mangleName ? fullName.InferredArity : fullName.ForcedArity,
                    mangleName,
                    isNativeInt: false,
                    errorInfo,
                    containingNamespace: null,
                    typeId)
            {
            }

            private TopLevel(ModuleSymbol module, string @namespace, string name, int arity, bool mangleName,
                bool isNativeInt, DiagnosticInfo? errorInfo, NamespaceSymbol? containingNamespace, int typeId)
                : base(name, arity, mangleName)
            {
                RoslynDebug.Assert((object)module != null);
                RoslynDebug.Assert(@namespace != null);
                RoslynDebug.Assert(typeId == -1 || typeId == (int)SpecialType.None || arity == 0 || mangleName);

                _namespaceName = @namespace;
                _containingModule = module;
                _isNativeInt = isNativeInt;
                _lazyErrorInfo = errorInfo;
                _lazyContainingNamespace = containingNamespace;
                _lazyTypeId = typeId;
            }


            /// <summary>
            /// This is the FULL namespace name (e.g., "System.Collections.Generic")
            /// of the type that couldn't be found.
            /// </summary>
            public string NamespaceName
            {
                get { return _namespaceName; }
            }

            internal override ModuleSymbol ContainingModule
            {
                get { return _containingModule; }
            }

            public override AssemblySymbol ContainingAssembly
            {
                get { return _containingModule.ContainingAssembly; }
            }

            public override Symbol ContainingSymbol
            {
                get
                {
                    if ((object?)_lazyContainingNamespace == null)
                    {
                        NamespaceSymbol container = _containingModule.GlobalNamespace;

                        if (_namespaceName.Length > 0)
                        {
                            var namespaces = MetadataHelpers.SplitQualifiedName(_namespaceName);
                            int i;

                            for (i = 0; i < namespaces.Length; i++)
                            {
                                NamespaceSymbol? newContainer = null;

                                foreach (NamespaceOrTypeSymbol symbol in container.GetMembers(namespaces[i]))
                                {
                                    if (symbol.Kind == SymbolKind.Namespace) // VB should also check name casing.
                                    {
                                        newContainer = (NamespaceSymbol)symbol;
                                        break;
                                    }
                                }

                                if ((object?)newContainer == null)
                                {
                                    break;
                                }

                                container = newContainer;
                            }

                            // now create symbols we couldn't find.
                            for (; i < namespaces.Length; i++)
                            {
                                container = new MissingNamespaceSymbol(container, namespaces[i]);
                            }
                        }

                        Interlocked.CompareExchange(ref _lazyContainingNamespace, container, null);
                    }

                    return _lazyContainingNamespace;
                }
            }

            private int TypeId
            {
                get
                {
                    if (_lazyTypeId == -1)
                    {
                        SpecialType typeId = SpecialType.None;

                        AssemblySymbol containingAssembly = _containingModule.ContainingAssembly;

                        if ((Arity == 0 || MangleName) && (object)containingAssembly != null &&
                            ReferenceEquals(containingAssembly, containingAssembly.CorLibrary))
                        {
                            // Check the name 
                            string emittedName = MetadataHelpers.BuildQualifiedName(_namespaceName, MetadataName);
                            typeId = SpecialTypes.GetTypeFromMetadataName(emittedName);
                        }

                        Interlocked.CompareExchange(ref _lazyTypeId, (int)typeId, -1);
                    }

                    return _lazyTypeId;
                }
            }

            public override SpecialType SpecialType
            {
                get
                {
                    int typeId = TypeId;
                    return (typeId >= (int)WellKnownType.First) ? SpecialType.None : (SpecialType)_lazyTypeId;
                }
            }


            public override int GetHashCode()
            {
                // Inherit special behavior for the object type from NamedTypeSymbol.
                if (this.SpecialType == Microsoft.CodeAnalysis.SpecialType.System_Object)
                {
                    return (int)Microsoft.CodeAnalysis.SpecialType.System_Object;
                }

                return Hash.Combine(MetadataName, Hash.Combine(_containingModule, Hash.Combine(_namespaceName, arity)));
            }


            private TopLevel AsNativeInteger(bool asNativeInt)
            {
                Debug.Assert(this.SpecialType == SpecialType.System_IntPtr ||
                             this.SpecialType == SpecialType.System_UIntPtr);

                if (asNativeInt == _isNativeInt)
                {
                    return this;
                }

                var other = new TopLevel(_containingModule, _namespaceName, name, arity, mangleName,
                    isNativeInt: asNativeInt, _lazyErrorInfo, _lazyContainingNamespace, _lazyTypeId);

                Debug.Assert(other.SpecialType == this.SpecialType);

                return other;
            }
        }

        /// <summary>
        /// Represents nested missing type.
        /// </summary>
        internal class Nested : MissingMetadataTypeSymbol
        {
            private readonly NamedTypeSymbol _containingType;

            public Nested(NamedTypeSymbol containingType, string name, int arity, bool mangleName)
                : base(name, arity, mangleName)
            {
                Debug.Assert((object)containingType != null);

                _containingType = containingType;
            }

            public Nested(NamedTypeSymbol containingType, ref MetadataTypeName emittedName)
                : this(containingType, ref emittedName,
                    emittedName.ForcedArity == -1 || emittedName.ForcedArity == emittedName.InferredArity)
            {
            }

            private Nested(NamedTypeSymbol containingType, ref MetadataTypeName emittedName, bool mangleName)
                : this(containingType,
                    mangleName ? emittedName.UnmangledTypeName : emittedName.TypeName,
                    mangleName ? emittedName.InferredArity : emittedName.ForcedArity,
                    mangleName)
            {
            }

            public override Symbol ContainingSymbol
            {
                get { return _containingType; }
            }


            public override SpecialType SpecialType
            {
                get
                {
                    return SpecialType.None; // do not have nested types among CORE types yet.
                }
            }

            public override int GetHashCode()
            {
                return Hash.Combine(_containingType, Hash.Combine(MetadataName, _arity));
            }

            internal override bool Equals(TypeSymbol t2, bool ignoreCustomModifiersAndArraySizesAndLowerBounds = false,
                bool ignoreDynamic = false)
            {
                if (ReferenceEquals(this, t2))
                {
                    return true;
                }

                var other = t2 as Nested;
                return (object)other != null &&
                       string.Equals(MetadataName, other.MetadataName, StringComparison.Ordinal) &&
                       _arity == other._arity &&
                       _containingType.Equals(other._containingType, ignoreCustomModifiersAndArraySizesAndLowerBounds,
                           ignoreDynamic);
            }
        }

        protected readonly string _name;
        protected readonly int _arity;
        protected readonly bool _mangleName;

        public MissingMetadataTypeSymbol(string name, int arity, bool mangleName)
        {
            Debug.Assert(name != null);

            this._name = name;
            this._arity = arity;
            this._mangleName = (mangleName && arity > 0);
        }

        public override CandidateReason CandidateReason => CandidateReason.None;

        public override string Name => _name;
        internal override bool MangleName => _mangleName;
        public override int Arity => _arity;
    }

    internal sealed class AmbiguousErrorTypeSymbol : ErrorTypeSymbol
    {
        internal ImmutableArray<NamedTypeSymbol> _candidates;

        public AmbiguousErrorTypeSymbol(ImmutableArray<NamedTypeSymbol> candidates)
        {
            Debug.Assert(!candidates.IsDefaultOrEmpty);
            _candidates = candidates;
        }

        public override CandidateReason CandidateReason => CandidateReason.Ambiguous;
        public override ImmutableArray<ISymbol> CandidateSymbols => _candidates.CastArray<ISymbol>();

        public override string Name => _candidates[0].Name;
        internal override bool MangleName => _candidates[0].MangleName;
        public override int Arity => 0;
    }
}