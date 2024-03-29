﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Threading;
using Aquila.CodeAnalysis.Symbols.Synthesized;
using Aquila.Syntax;
using Microsoft.CodeAnalysis;
using Aquila.CodeAnalysis.Semantics;
using Aquila.CodeAnalysis.Syntax;

namespace Aquila.CodeAnalysis.Symbols.Source
{
    /// <summary>
    /// Represents a Aquila function parameter.
    /// </summary>
    internal sealed class SourceParameterSymbol : ParameterSymbol
    {
        readonly SourceMethodSymbolBase _method;
        readonly ParameterSyntax _syntax;

        /// <summary>
        /// Index of the source parameter, relative to the first source parameter.
        /// </summary>
        readonly int _relindex;

        TypeSymbol _lazyType;

        /// <summary>
        /// Optional. The parameter initializer expression i.e. bound <see cref="FormalParam.InitValue"/>.
        /// </summary>
        public override BoundExpression Initializer => _initializer;

        readonly BoundExpression _initializer;

        /// <summary>
        /// Whether the parameter needs to be copied when passed by value.
        /// Can be set to <c>false</c> by analysis (e.g. unused parameter or only delegation to another method).
        /// </summary>
        public bool CopyOnPass { get; set; } = true;

        public override FieldSymbol DefaultValueField
        {
            get
            {
                if (_lazyDefaultValueField == null && Initializer != null && ExplicitDefaultConstantValue == null)
                {
                    TypeSymbol fldtype; // type of the field

                    if (Initializer is BoundArrayEx arr)
                    {
                        fldtype = null;
                    }
                    else
                    {
                        fldtype = null;
                    }

                    // The construction of the default value may require a Context, cannot be created as a static singletong
                    if (Initializer.RequiresContext ||
                        (fldtype.IsReferenceType &&
                         fldtype.SpecialType != SpecialType.System_String)
                       ) // we can cache the default value even for Refs if it is an immutable value
                    {
                        fldtype = DeclaringCompilation.GetWellKnownType(WellKnownType.System_Func_T2).Construct(
                            null,
                            null);
                    }

                    // determine the field container:
                    NamedTypeSymbol fieldcontainer = ContainingType; // by default in the containing class/trait/file
                    string fieldname = $"<{ContainingSymbol.Name}.{Name}>_DefaultValue";

    
                    var field = new SynthesizedFieldSymbol(fieldcontainer)
                        .SetType(fldtype)
                        .SetName(fieldname)
                        .SetAccess(Accessibility.Public)
                        .SetIsStatic(true)
                        .SetReadOnly(true);

                    //
                    Interlocked.CompareExchange(ref _lazyDefaultValueField, field, null);
                }

                return _lazyDefaultValueField;
            }
        }

        FieldSymbol _lazyDefaultValueField;

        public SourceParameterSymbol(SourceMethodSymbolBase method, ParameterSyntax syntax, int relindex)
        {
            Contract.ThrowIfNull(method);
            Contract.ThrowIfNull(syntax);
            Debug.Assert(relindex >= 0);

            _method = method;
            _syntax = syntax;
            _relindex = relindex;
        }

        /// <summary>
        /// Containing method.
        /// </summary>
        internal SourceMethodSymbolBase Method => _method;

        public override Symbol ContainingSymbol => _method;

        internal override AquilaCompilation DeclaringCompilation => _method.DeclaringCompilation;

        internal override ModuleSymbol ContainingModule => _method.ContainingModule;

        public override NamedTypeSymbol ContainingType => _method.ContainingType;

        public override string Name => _syntax.Identifier.Text;

        public override bool IsThis => false;

        public ParameterSyntax Syntax => _syntax;

        internal sealed override TypeSymbol Type
        {
            get
            {
                if (_lazyType == null)
                {
                    Interlocked.CompareExchange(ref _lazyType, ResolveType(), null);
                }

                return _lazyType;
            }
        }
        internal bool DefaultsToNull => _initializer != null && _initializer.ConstantValue.IsNull();

        /// <summary>
        /// Gets value indicating whether the parameter has been replaced with <see cref="SourceMethodSymbol.VarargsParam"/>.
        /// </summary>
        internal bool IsFake => (Method.GetParamsParameter() != null && Method.GetParamsParameter() != this &&
                                 Ordinal >= Method.GetParamsParameter().Ordinal);

        TypeSymbol ResolveType()
        {
            if (IsThis)
            {
                return ContainingType;
            }

            return DeclaringCompilation.GetBinder(_syntax).BindType(_syntax.Type);
        }

        public override RefKind RefKind =>RefKind.None;
        public override int Ordinal => _relindex + _method.ImplicitParameters.Length;

        /// <summary>
        /// Zero-based index of the source parameter.
        /// </summary>
        public int ParameterIndex => _relindex;

        public override ImmutableArray<Location> Locations
        {
            get
            {
                return ImmutableArray.Create(Location.Create(Method.Syntax.SyntaxTree,
                    _syntax.Identifier.Span));
            }
        }

        public override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences
        {
            get { throw new NotImplementedException(); }
        }

        internal override IEnumerable<AttributeData> GetCustomAttributesToEmit(
            CommonModuleCompilationState compilationState)
        {
            // [param]   
            if (IsParams)
            {
                yield return DeclaringCompilation.CreateParamsAttribute();
            }

            // [NotNull]
            if (HasNotNull && Type.IsReferenceType)
            {
                yield return DeclaringCompilation.CreateNotNullAttribute();
            }

            // [DefaultValue]
            if (DefaultValueField != null)
            {
                yield return DeclaringCompilation.CreateDefaultValueAttribute(ContainingType, DefaultValueField);
            }
        }

        

        internal override ConstantValue ExplicitDefaultConstantValue
        {
            get
            {
                ConstantValue value = null;

                if (Initializer != null)
                {
                    // NOTE: the constant does not have to have the exact same type as the parameter, it is up to the caller of the method to process DefaultValue and convert it if necessary

                    value = Initializer.ConstantValue.ToConstantValueOrNull();
                    if (value != null)
                    {
                        return value;
                    }

                    // NOTE: non-literal default values (like array()) must be handled by creating a ghost method overload calling this method:

                    // Template:
                    // foo($a = [], $b = [1, 2, 3]) =>
                    // + foo($a, $b){ /* this method */ }
                    // + foo($a) => foo($a, [1, 2, 3])
                    // + foo() => foo([], [1, 2, 3)
                }

                //
                return value;
            }
        }
    }
}