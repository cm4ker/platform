using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.PooledObjects;
using Aquila.CodeAnalysis.Semantics;

namespace Aquila.CodeAnalysis.Symbols.Synthesized
{
    /// <summary>
    /// Represents a simple compiler generated parameter of a given type.
    /// </summary>
    internal class SynthesizedParameterSymbol : ParameterSymbol
    {
        private readonly MethodSymbol _container;
        private readonly TypeSymbol _type;
        private readonly string _name;
        private readonly bool _isParams;
        private readonly ImmutableArray<CustomModifier> _customModifiers;
        private readonly ushort _countOfCustomModifiersPrecedingByRef;
        private readonly RefKind _refKind;
        private readonly ConstantValue _explicitDefaultConstantValue;

        private int _ordinal;

        public override BoundExpression Initializer => null;

        public override FieldSymbol DefaultValueField { get; }

        public SynthesizedParameterSymbol(
            MethodSymbol container,
            TypeSymbol type,
            int ordinal,
            RefKind refKind,
            string name = "",
            bool isParams = false,
            ImmutableArray<CustomModifier> customModifiers = default(ImmutableArray<CustomModifier>),
            ushort countOfCustomModifiersPrecedingByRef = 0,
            ConstantValue explicitDefaultConstantValue = null,
            FieldSymbol defaultValueField = null)
        {
            Debug.Assert(container != null);
            Debug.Assert((object)type != null);
            Debug.Assert(name != null);
            Debug.Assert(ordinal >= 0);

            _container = container;
            _type = type;
            _ordinal = ordinal;
            _refKind = refKind;
            _name = name;
            _isParams = isParams;
            _customModifiers = customModifiers.NullToEmpty();
            _countOfCustomModifiersPrecedingByRef = countOfCustomModifiersPrecedingByRef;
            _explicitDefaultConstantValue = explicitDefaultConstantValue;

            this.DefaultValueField = defaultValueField;
        }

        public static SynthesizedParameterSymbol Create(MethodSymbol container, ParameterSymbol p,
            int? ordinal = default)
        {
            var defaultValueField = ((ParameterSymbol)p.OriginalDefinition).DefaultValueField;

            return new SynthesizedParameterSymbol(container, p.Type, ordinal.HasValue ? ordinal.Value : p.Ordinal,
                p.RefKind,
                name: p.Name,
                isParams: p.IsParams,
                explicitDefaultConstantValue: p.ExplicitDefaultConstantValue,
                defaultValueField: defaultValueField);
        }

        public static ImmutableArray<ParameterSymbol> Create(MethodSymbol container,
            ImmutableArray<ParameterSymbol> srcparams)
        {
            if (srcparams.Length != 0)
            {
                var builder = ImmutableArray.CreateBuilder<ParameterSymbol>(srcparams.Length);

                foreach (var p in srcparams)
                {
                    builder.Add(Create(container, p));
                }

                return builder.MoveToImmutable();
            }
            else
            {
                return ImmutableArray<ParameterSymbol>.Empty;
            }
        }

        internal override TypeSymbol Type
        {
            get { return _type; }
        }

        public override RefKind RefKind
        {
            get { return _refKind; }
        }

        public override string Name
        {
            get { return _name; }
        }

        public override ImmutableArray<CustomModifier> CustomModifiers
        {
            get { return _customModifiers; }
        }

        internal override IEnumerable<AttributeData> GetCustomAttributesToEmit(
            CommonModuleCompilationState compilationState)
        {
            // params
            if (IsParams)
            {
                yield return DeclaringCompilation.CreateParamsAttribute();
            }

            // TODO: preserve [NotNull]

            // [DefaultValue]
            if (DefaultValueField != null)
            {
                yield return DeclaringCompilation.CreateDefaultValueAttribute(ContainingType, DefaultValueField);
            }

            //
            yield break;
        }

        public override int Ordinal => _ordinal;

        internal void UpdateOrdinal(int newordinal)
        {
            _ordinal = newordinal;
        }

        public override bool IsParams => _isParams;
        
        public override bool IsImplicitlyDeclared
        {
            get
            {
                return
                    SpecialParameterSymbol.IsContextParameter(this) ||
                    SpecialParameterSymbol.IsImportValueParameter(this) ||
                    SpecialParameterSymbol.IsDummyFieldsOnlyCtorParameter(this) ||
                    SpecialParameterSymbol.IsLateStaticParameter(this) ||
                    SpecialParameterSymbol.IsSelfParameter(this) ||
                    this.IsParams ||
                    base.IsImplicitlyDeclared;
            }
        }

        internal override ConstantValue ExplicitDefaultConstantValue => _explicitDefaultConstantValue;

        public override bool IsOptional => _explicitDefaultConstantValue != null;

        public sealed override ushort CountOfCustomModifiersPrecedingByRef
        {
            get { return _countOfCustomModifiersPrecedingByRef; }
        }

        public override Symbol ContainingSymbol
        {
            get { return _container; }
        }

        public override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences
        {
            get { return ImmutableArray<SyntaxReference>.Empty; }
        }

        /// <summary>
        /// For each parameter of a source method, construct a corresponding synthesized parameter
        /// for a destination method.
        /// </summary>
        /// <param name="sourceMethod">Has parameters.</param>
        /// <param name="destinationMethod">Needs parameters.</param>
        /// <returns>Synthesized parameters to add to destination method.</returns>
        internal static ImmutableArray<ParameterSymbol> DeriveParameters(MethodSymbol sourceMethod,
            MethodSymbol destinationMethod)
        {
            var builder = ArrayBuilder<ParameterSymbol>.GetInstance();

            foreach (var oldParam in sourceMethod.Parameters)
            {
                //same properties as the old one, just change the owner
                builder.Add(new SynthesizedParameterSymbol(destinationMethod, oldParam.Type, oldParam.Ordinal,
                    oldParam.RefKind, oldParam.Name, false, oldParam.CustomModifiers,
                    oldParam.CountOfCustomModifiersPrecedingByRef));
            }

            return builder.ToImmutableAndFree();
        }
    }
}