using System;
using System.Collections.Immutable;
using System.Globalization;
using System.Threading;
using Aquila.CodeAnalysis.Symbols.Synthesized;
using Microsoft.CodeAnalysis;
using Aquila.CodeAnalysis.Semantics;
using Roslyn.Utilities;

namespace Aquila.CodeAnalysis.Symbols
{
    /// <summary>
    /// Declares a CLR field representing a Aquila field (a class constant or a field).
    /// </summary>
    /// <remarks>
    /// Its CLR properties vary depending on <see cref="SourceFieldSymbol.Initializer"/> and its evaluation.
    /// Some expressions have to be evaluated in runtime which causes the field to be contained in <see cref="Synthesized.SynthesizedStaticFieldsHolder"/>.
    /// </remarks>
    internal partial class SourceFieldSymbol : FieldSymbol
    {
        //readonly SourceTypeSymbol _containingType;
        readonly string _fieldName;

        readonly Location _location;

        /// <summary>
        /// Declared accessibility - private, protected or public.
        /// </summary>
        readonly Accessibility _accessibility;

        readonly BoundExpression _initializer;

        readonly ImmutableArray<AttributeData> _customAttributes;

        /// <summary>
        /// Gets value indicating whether this field redefines a field from a base type.
        /// </summary>
        public bool IsRedefinition => !ReferenceEquals(OverridenDefinition, null);

        /// <summary>
        /// Gets field from a base type that is redefined by this field.
        /// </summary>
        public FieldSymbol OverridenDefinition
        {
            get
            {
                if (ReferenceEquals(_originaldefinition, null))
                {
                    // resolve overriden field symbol
                    _originaldefinition = ResolveOverridenDefinition() ?? this;
                }

                return ReferenceEquals(_originaldefinition, this) ? null : _originaldefinition;
            }
        }

        FieldSymbol _originaldefinition;

        FieldSymbol ResolveOverridenDefinition()
        {
            return null;
        }

        /// <summary>
        /// Optional property that provides public access to <see cref="OverridenDefinition"/> if it is protected.
        /// </summary>
        public PropertySymbol FieldAccessorProperty
        {
            get
            {
                if (IsRedefinition && OverridenDefinition.DeclaredAccessibility < this.DeclaredAccessibility &&
                    _fieldAccessorProperty == null)
                {
                    // declare property accessing the field from outside:
                    var type = OverridenDefinition.Type;

                    // TYPE get_NAME()
                    var getter = new SynthesizedMethodSymbol(this.ContainingType, "get_" + this.Name, false, false,
                        type, this.DeclaredAccessibility);

                    // void set_NAME(TYPE `value`)
                    var setter = new SynthesizedMethodSymbol(this.ContainingType, "set_" + this.Name, false, false,
                        DeclaringCompilation.CoreTypes.Void, this.DeclaredAccessibility);
                    setter.SetParameters(new SynthesizedParameterSymbol(setter, type, 0, RefKind.None, "value"));

                    // TYPE NAME { get; set; }
                    var fieldAccessorProperty =
                        new SynthesizedPropertySymbol(this.ContainingType)
                            .SetName(this.Name)
                            .SetType(type)
                            .SetGetMethod(getter)
                            .SetSetMethod(setter)
                            .SetAccess(DeclaredAccessibility)
                            .SetIsStatic(false);
                    Interlocked.CompareExchange(ref _fieldAccessorProperty, fieldAccessorProperty, null);
                }

                return _fieldAccessorProperty;
            }
        }

        PropertySymbol _fieldAccessorProperty;


        #region FieldSymbol

        public override string Name => _fieldName;

        public override Symbol AssociatedSymbol => null;

        public override Symbol ContainingSymbol => null;

        internal override AquilaCompilation DeclaringCompilation => null; //_containingType.DeclaringCompilation;

        public override ImmutableArray<CustomModifier> CustomModifiers => ImmutableArray<CustomModifier>.Empty;

        public override Accessibility DeclaredAccessibility => _accessibility;

        public override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences
        {
            get { throw new NotImplementedException(); }
        }

        public override bool IsVolatile => false;

        public override ImmutableArray<Location> Locations => ImmutableArray.Create(_location);

        internal override bool HasRuntimeSpecialName => false;

        internal override bool HasSpecialName => false;

        internal override bool IsNotSerialized => false;

        internal override MarshalPseudoCustomAttributeData MarshallingInformation => null;

        internal override ObsoleteAttributeData ObsoleteAttributeData => null;

        internal override int? TypeLayoutOffset => null;

        public override ImmutableArray<AttributeData> GetAttributes()
        {
            var attrs = _customAttributes;

            // attributes from syntax node
            if (attrs.IsDefaultOrEmpty)
            {
                attrs = ImmutableArray<AttributeData>.Empty;
            }
            else
            {
                // // initialize attribute data if necessary:
                // attrs
                //     .OfType<SourceCustomAttribute>()
                //     .ForEach(x => x.Bind(this, _containingType.ContainingFile));
            }

            return attrs;
        }

        #endregion

        internal override ConstantValue GetConstantValue(bool earlyDecodingWellKnownAttributes)
        {
            return null;
        }

        internal override TypeSymbol GetFieldType(ConsList<FieldSymbol> fieldsBeingBound)
        {
            if ((IsConst || IsReadOnly) && Initializer != null)
            {
                // resolved type symbol if possible
                if (Initializer.ResultType != null)
                {
                    return Initializer.ResultType;
                }

                // resolved value type if possible
                var cvalue = Initializer.ConstantValue;
                if (cvalue.HasValue)
                {
                    var specialType = (cvalue.Value != null)
                        ? cvalue.ToConstantValueOrNull()?.SpecialType
                        : SpecialType.System_Object; // NULL

                    if (specialType.HasValue && specialType != SpecialType.None)
                    {
                        return DeclaringCompilation.GetSpecialType(specialType.Value);
                    }
                }

                //
                //return DeclaringCompilation.GetTypeFromTypeRef(typectx, Initializer.TypeRefMask);
            }

            // default
            return null;
        }

        /// <summary>
        /// <c>const</c> whether the field is a constant and its value can be resolved as constant value.
        /// </summary>
        public override bool IsConst => GetConstantValue(false) != null;

        /// <summary>
        /// <c>readonly</c> applies to class constants that have to be evaluated at runtime.
        /// </summary>
        public override bool IsReadOnly => false;

        /// <summary>
        /// Whether the field is real CLR static field.
        /// </summary>
        public override bool IsStatic =>
            IsConst;


        public override string GetDocumentationCommentXml(CultureInfo preferredCulture = null,
            bool expandIncludes = false, CancellationToken cancellationToken = default(CancellationToken))
        {
            var summary = string.Empty;
            return summary;
        }
    }
}