﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Aquila.Compiler.Utilities;
using Microsoft.CodeAnalysis;
using Aquila.CodeAnalysis;
using Aquila.CodeAnalysis.CodeGen;
using Aquila.CodeAnalysis.Emit;
using Roslyn.Utilities;

namespace Aquila.CodeAnalysis.Symbols.Synthesized
{
    /// <summary>
    /// Nested class representing a static local variable.
    /// 
    /// Template:
    /// class static'foo'x : IStaticInit {
    ///     public T value;
    ///     public void Init(Context ctx){ ... }
    /// }
    /// </summary>
    internal class SynthesizedStaticLocHolder : NamedTypeSymbol
    {
        /// <summary>
        /// Containing source method.
        /// </summary>
        public IMethodSymbol DeclaringMethod => _method;

        readonly SourceMethodSymbolBase _method;

        public override NamedTypeSymbol ContainingType => base.ContainingType;

        /// <summary>
        /// Name of the local variable.
        /// </summary>
        public string VariableName => _locName;

        readonly string _locName;

        /// <summary>
        /// Type of local variable.
        /// </summary>
        public TypeSymbol ValueType => _locType ?? null;

        readonly TypeSymbol _locType;

        public override bool IsImplicitlyDeclared => true;

        public override ImmutableArray<NamedTypeSymbol> Interfaces
        {
            get { return base.Interfaces; }
        }

        /// <summary>
        /// The containing value represented as a class field.
        /// </summary>
        public SynthesizedFieldSymbol ValueField
        {
            get
            {
                if (_valueField == null)
                {
                    var valueField = new SynthesizedFieldSymbol(this)
                        .SetName("value")
                        .SetAccess(Accessibility.Public)
                        .SetIsStatic(false)
                        .SetType(ValueType);


                    Interlocked.CompareExchange(ref _valueField, valueField, null);
                }

                return _valueField;
            }
        }

        SynthesizedFieldSymbol _valueField;

        public void EmitCtor(PEModuleBuilder module,
            Action<Microsoft.CodeAnalysis.CodeGen.ILBuilder> builder)
        {
            Debug.Assert(_ctor == null);

            // emit default .ctor

            _ctor = new SynthesizedCtorSymbol(this);
            _ctor.SetParameters(); // empty params (default ctor)

            var body = MethodGenerator.GenerateMethodBody(module, _ctor, builder, null,
                DiagnosticBag.GetInstance(), false);
            module.SetMethodBody(_ctor, body);
        }

        SynthesizedCtorSymbol _ctor;

        public void EmitInit(PEModuleBuilder module,
            Action<Microsoft.CodeAnalysis.CodeGen.ILBuilder> builder)
        {
            Debug.Assert(_initMethod == null);

            var tt = DeclaringCompilation.CoreTypes;

            // override IStaticInit.Init(Context)

            _initMethod = new SynthesizedMethodSymbol(this, "Init", false, true, tt.Void, Accessibility.Public);
            _initMethod.SetParameters(new SynthesizedParameterSymbol(_initMethod, null, 0, RefKind.None, "ctx"));

            var body = MethodGenerator.GenerateMethodBody(module, _initMethod, builder,
                null, DiagnosticBag.GetInstance(), false);
            module.SetMethodBody(_initMethod, body);
        }

        SynthesizedMethodSymbol _initMethod;

        public SynthesizedStaticLocHolder(SourceMethodSymbolBase method, string locName, TypeSymbol locType = null)
        {
            Contract.ThrowIfNull(method);

            _method = method;
            _locName = locName ?? "?";
            _locType = locType;
        }

        public override int Arity => 0;

        internal override bool HasTypeArgumentsCustomModifiers => false;

        public override ImmutableArray<CustomModifier> GetTypeArgumentCustomModifiers(int ordinal) =>
            GetEmptyTypeArgumentCustomModifiers(ordinal);

        public override Symbol ContainingSymbol => _method.ContainingType;

        internal override AquilaCompilation DeclaringCompilation => _method.DeclaringCompilation;

        public override NamedTypeSymbol BaseType => DeclaringCompilation.CoreTypes.Object;

        public override Accessibility DeclaredAccessibility => Accessibility.Private;

        public override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences
        {
            get { throw new NotImplementedException(); }
        }

        public override bool IsAbstract => false;

        public override bool IsSealed => true;

        public override bool IsStatic => false;

        public override bool IsSerializable => false;

        public override string Name => _method.MetadataName.Replace('.', '-') + "$" + _locName;

        public override TypeKind TypeKind => TypeKind.Class;

        internal override bool IsInterface => false;

        internal override bool IsWindowsRuntimeImport => false;

        internal override TypeLayout Layout => default(TypeLayout);

        internal override bool MangleName => false;

        internal override ObsoleteAttributeData ObsoleteAttributeData => null;

        internal override bool ShouldAddWinRTMembers => false;

        public override ImmutableArray<Symbol> GetMembers()
        {
            Debug.Assert(_ctor != null);

            var list = new List<Symbol>()
            {
                ValueField,
                _ctor
            };

            if (_initMethod != null)
            {
                list.Add(_initMethod);
            }

            return list.AsImmutable();
        }

        public override ImmutableArray<Symbol> GetMembers(string name)
        {
            return GetMembers().Where(s => s.Name.StringsEqual(name, ignoreCase: false)).AsImmutable();
        }

        public override ImmutableArray<MethodSymbol> Constructors => ImmutableArray.Create((MethodSymbol)_ctor);

        public override ImmutableArray<MethodSymbol> InstanceConstructors =>
            Constructors.Where(s => !s.IsStatic).AsImmutable();

        public override ImmutableArray<NamedTypeSymbol> GetTypeMembers() => ImmutableArray<NamedTypeSymbol>.Empty;

        public override ImmutableArray<NamedTypeSymbol> GetTypeMembers(string name) =>
            ImmutableArray<NamedTypeSymbol>.Empty;

        internal override ImmutableArray<NamedTypeSymbol> GetDeclaredInterfaces(ConsList<Symbol> basesBeingResolved) =>
            GetInterfacesToEmit();

        internal override IEnumerable<IFieldSymbol> GetFieldsToEmit()
        {
            return new IFieldSymbol[] { ValueField };
        }

        internal override ImmutableArray<NamedTypeSymbol> GetInterfacesToEmit()
        {
            if (_initMethod != null)
            {
                return ImmutableArray.Create((NamedTypeSymbol)null);
            }
            else
            {
                return ImmutableArray<NamedTypeSymbol>.Empty;
            }
        }

        public override ImmutableArray<NamedTypeSymbol> AllInterfaces => GetInterfacesToEmit();
    }
}