using System;
using System.Collections.Immutable;
using System.ComponentModel.Design;
using System.Globalization;
using System.Threading;
using Aquila.CodeAnalysis.Symbols.Synthesized;
using Microsoft.CodeAnalysis;
using Aquila.CodeAnalysis.Semantics;
using Aquila.CodeAnalysis.Symbols.Source;
using Aquila.CodeAnalysis.Syntax;
using Roslyn.Utilities;

namespace Aquila.CodeAnalysis.Symbols
{
    /// <summary>
    /// Declares a CLR field representing a Aquila field (a class constant or a field).
    /// </summary>
    /// <remarks>
    /// Its CLR properties vary depending on <see cref="SourceFieldSymbol.Initializer"/> and its evaluation.
    /// Some expressions have to be evaluated in runtime which causes the field to be contained in <see cref="SynthesizedStaticFieldsHolder"/>.
    /// </remarks>
    internal partial class SourceFieldSymbol : FieldSymbol
    {
        private readonly FieldDecl _fieldSyntax;
        readonly NamedTypeSymbol _containingType;


        public SourceFieldSymbol(FieldDecl fieldSyntax, NamedTypeSymbol containingType)
        {
            _fieldSyntax = fieldSyntax;
            _containingType = containingType;
        }

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

                    var getter = new SynthesizedMethodSymbol(this.ContainingType, "get_" + this.Name, false, false,
                        type, this.DeclaredAccessibility);

                    var setter = new SynthesizedMethodSymbol(this.ContainingType, "set_" + this.Name, false, false,
                        DeclaringCompilation.CoreTypes.Void, this.DeclaredAccessibility);
                    setter.SetParameters(new SynthesizedParameterSymbol(setter, type, 0, RefKind.None, "value"));

                    
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

        public override string Name => _fieldSyntax.Identifier.Text;

        public override Symbol AssociatedSymbol => null;

        public override Symbol ContainingSymbol => _containingType;

        internal override AquilaCompilation DeclaringCompilation => _containingType.DeclaringCompilation;

        public override ImmutableArray<CustomModifier> CustomModifiers => ImmutableArray<CustomModifier>.Empty;

        public override Accessibility DeclaredAccessibility => _accessibility;

        public override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences
        {
            get { throw new NotImplementedException(); }
        }

        public override bool IsVolatile => false;

        public override ImmutableArray<Location> Locations => ImmutableArray.Create(_fieldSyntax.Location);

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

            return attrs;
        }

        #endregion

        internal override ConstantValue GetConstantValue(bool earlyDecodingWellKnownAttributes)
        {
            return null;
        }

        internal override TypeSymbol GetFieldType(ConsList<FieldSymbol> fieldsBeingBound)
        {
            var binder = DeclaringCompilation.GetBinder(_fieldSyntax);
            return binder.BindType(_fieldSyntax.Type);
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