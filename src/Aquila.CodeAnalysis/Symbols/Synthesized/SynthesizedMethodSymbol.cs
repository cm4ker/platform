using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using Aquila.CodeAnalysis.Symbols.Attributes;
using Microsoft.CodeAnalysis;
using Cci = Microsoft.Cci;

namespace Aquila.CodeAnalysis.Symbols.Synthesized
{
    partial class SynthesizedMethodSymbol : MethodSymbol
    {
        readonly Cci.ITypeDefinition _containing;
        readonly ModuleSymbol _module;
        bool _static, _virtual, _final, _abstract;
        string _name;
        TypeSymbol _return;
        Accessibility _accessibility;
        protected ImmutableArray<ParameterSymbol> _parameters;

        /// <summary>
        /// Optional.
        /// Gats actual method that will be called by this one.
        /// For informational purposes.
        /// </summary>
        public MethodSymbol ForwardedCall { get; set; }

        internal MethodSymbol ExplicitOverride { get; set; }

        /// <summary>
        /// If set to <c>true</c>, the method will emit [EditorBrowsable(Never)] attribute.
        /// </summary>
        public bool IsEditorBrowsableHidden { get; internal set; }

        public override bool IsAquilaHidden => IsAquilaHiddenInternal;

        internal bool IsAquilaHiddenInternal { get; set; }

        public override IMethodSymbol OverriddenMethod => ExplicitOverride;

        public SynthesizedMethodSymbol(Cci.ITypeDefinition containingType, ModuleSymbol module, string name,
            bool isstatic, bool isvirtual, TypeSymbol returnType, Accessibility accessibility = Accessibility.Private,
            bool isfinal = true, bool isabstract = false)
        {
            _containing = containingType ?? throw new ArgumentNullException(nameof(containingType));
            _module = module ?? throw new ArgumentNullException(nameof(module));
            _name = name;
            _static = isstatic;
            _virtual = isvirtual && !isstatic;
            _abstract = isvirtual && isabstract && !isfinal;
            _return = returnType;
            _accessibility = accessibility;
            _final = isfinal && isvirtual && !isstatic;
        }

        public SynthesizedMethodSymbol(TypeSymbol containingType)
        {
            _containing = containingType as Cci.ITypeDefinition ??
                          throw new ArgumentNullException(nameof(containingType));
            _module = (ModuleSymbol)containingType.ContainingModule ??
                      throw new NullReferenceException("Module is null");

            _return = DeclaringCompilation.CoreTypes.Void;
            _parameters = ImmutableArray<ParameterSymbol>.Empty;
        }

        public SynthesizedMethodSymbol(TypeSymbol containingType, string name, bool isstatic, bool isvirtual,
            TypeSymbol returnType, Accessibility accessibility = Accessibility.Private, bool isfinal = true,
            bool isabstract = false, bool aquilahidden = false, params ParameterSymbol[] ps)
            : this(containingType as Cci.ITypeDefinition, (ModuleSymbol)containingType.ContainingModule, name,
                isstatic, isvirtual, returnType, accessibility, isfinal, isabstract)
        {
            IsAquilaHiddenInternal = aquilahidden;
            SetParameters(ps);
        }

        #region Setters

        internal SynthesizedMethodSymbol SetParameters(params ParameterSymbol[] ps) => SetParameters(ps.AsImmutable());

        internal SynthesizedMethodSymbol SetParameters(ImmutableArray<ParameterSymbol> ps)
        {
            Debug.Assert(!ps.IsDefault);
            _parameters = ps;
            return this;
        }

        internal SynthesizedMethodSymbol SetIsStatic(bool value)
        {
            _static = value;
            return this;
        }

        internal SynthesizedMethodSymbol SetVirtual(bool value)
        {
            _virtual = value && !_static;
            return this;
        }

        internal SynthesizedMethodSymbol SetAbstract(bool value)
        {
            _abstract = _virtual && value && !_final;
            return this;
        }

        internal SynthesizedMethodSymbol SetIsFinal(bool value)
        {
            _final = value && _virtual && !_static;
            return this;
        }

        internal SynthesizedMethodSymbol SetReturn(TypeSymbol type)
        {
            _return = type;
            return this;
        }

        internal SynthesizedMethodSymbol SetAccess(Accessibility value)
        {
            _accessibility = value;
            return this;
        }

        internal SynthesizedMethodSymbol SetName(string value)
        {
            _name = value;
            return this;
        }

        #endregion

        public override ImmutableArray<AttributeData> GetAttributes()
        {
            var builder = ImmutableArray.CreateBuilder<AttributeData>();

            if (IsAquilaHidden)
            {
                // [AquilaHiddenAttribute]
                builder.Add(new SynthesizedAttributeData(
                    null,
                    ImmutableArray<TypedConstant>.Empty,
                    ImmutableArray<KeyValuePair<string, TypedConstant>>.Empty));
            }

            if (IsEditorBrowsableHidden)
            {
                builder.Add(new SynthesizedAttributeData(
                    (MethodSymbol)DeclaringCompilation.GetWellKnownTypeMember(WellKnownMember
                        .System_ComponentModel_EditorBrowsableAttribute__ctor),
                    ImmutableArray.Create(
                        new TypedConstant(
                            DeclaringCompilation.GetWellKnownType(WellKnownType
                                .System_ComponentModel_EditorBrowsableState),
                            TypedConstantKind.Enum,
                            System.ComponentModel.EditorBrowsableState.Never)),
                    ImmutableArray<KeyValuePair<string, TypedConstant>>.Empty));
            }

            return builder.ToImmutable();
        }

        public override Symbol ContainingSymbol => _containing as Symbol;

        internal override ModuleSymbol ContainingModule => _module;

        internal override AquilaCompilation DeclaringCompilation => _module.DeclaringCompilation;

        public override Accessibility DeclaredAccessibility => _accessibility;

        public override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences
        {
            get { throw new NotImplementedException(); }
        }

        public override bool IsAbstract => _abstract;

        public override bool IsExtern => false;

        public override bool IsOverride => OverriddenMethod != null &&
                                           (OverriddenMethod.ContainingType.TypeKind != TypeKind.Interface);

        public override bool IsSealed => _final;

        public override bool IsStatic => _static;

        public override bool IsVirtual => _virtual;

        public override bool IsImplicitlyDeclared => true;

        public override ImmutableArray<MethodSymbol> ExplicitInterfaceImplementations =>
            IsExplicitInterfaceImplementation
                ? ImmutableArray.Create(ExplicitOverride)
                : ImmutableArray<MethodSymbol>.Empty;

        internal override bool IsExplicitInterfaceImplementation =>
            ExplicitOverride != null && ExplicitOverride.ContainingType.IsInterface;

        public override MethodKind MethodKind
        {
            get
            {
                Debug.Assert(
                    Name != WellKnownMemberNames.InstanceConstructorName &&
                    Name != WellKnownMemberNames.StaticConstructorName);

                return MethodKind.Ordinary;
            }
        }

        public override string Name => _name;

        public override ImmutableArray<ParameterSymbol> Parameters => _parameters;

        public override bool ReturnsVoid => ReturnType.SpecialType == SpecialType.System_Void;

        public override RefKind RefKind => RefKind.None;

        public override TypeSymbol ReturnType =>
            _return ?? ForwardedCall?.ReturnType ?? throw new InvalidOperationException();

        internal override ObsoleteAttributeData ObsoleteAttributeData => null;

        public override bool HidesBaseMethodsByName => !IsExplicitInterfaceImplementation && true;

        /// <summary>
        /// virtual = IsVirtual AND NewSlot 
        /// override = IsVirtual AND !NewSlot
        /// </summary>
        internal override bool IsMetadataNewSlot(bool ignoreInterfaceImplementationChanges = false) =>
            IsVirtual && !IsOverride;

        internal override bool IsMetadataVirtual(bool ignoreInterfaceImplementationChanges = false) => IsVirtual;
    }
}