﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Pchp.CodeAnalysis;
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
        /// Containing source routine.
        /// </summary>
        public IMethodSymbol DeclaringMethod => _routine;
        readonly Source.SourceRoutineSymbol _routine;

        public override NamedTypeSymbol ContainingType => base.ContainingType;

        /// <summary>
        /// Name of the local variable.
        /// </summary>
        public string VariableName => _locName;
        readonly string _locName;

        /// <summary>
        /// Type of local variable.
        /// <c>PhpAlias</c> by default.
        /// </summary>
        public TypeSymbol ValueType => _locType ?? DeclaringCompilation.CoreTypes.PhpAlias;
        readonly TypeSymbol _locType;

        public override bool IsImplicitlyDeclared => true;

        public override ImmutableArray<NamedTypeSymbol> Interfaces
        {
            get
            {
                return base.Interfaces;
            }
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
                    var valueField = new SynthesizedFieldSymbol(this, ValueType, "value", Accessibility.Public, false);
                    Interlocked.CompareExchange(ref _valueField, valueField, null);
                }

                return _valueField;
            }
        }
        SynthesizedFieldSymbol _valueField;

        public void EmitCtor(Pchp.CodeAnalysis.Emit.PEModuleBuilder module, Action<Microsoft.CodeAnalysis.CodeGen.ILBuilder> builder)
        {
            Debug.Assert(_ctor == null);

            // emit default .ctor

            _ctor = new SynthesizedCtorSymbol(this);
            _ctor.SetParameters();// empty params (default ctor)
            
            var body = Pchp.CodeAnalysis.CodeGen.MethodGenerator.GenerateMethodBody(module, _ctor, builder, null, DiagnosticBag.GetInstance(), false);
            module.SetMethodBody(_ctor, body);
        }
        SynthesizedCtorSymbol _ctor;

        public void EmitInit(Pchp.CodeAnalysis.Emit.PEModuleBuilder module, Action<Microsoft.CodeAnalysis.CodeGen.ILBuilder> builder)
        {
            Debug.Assert(_initMethod == null);

            var tt = DeclaringCompilation.CoreTypes;

            // override IStaticInit.Init(Context)

            _initMethod = new SynthesizedMethodSymbol(this, "Init", false, true, tt.Void, Accessibility.Public);
            _initMethod.SetParameters(new SynthesizedParameterSymbol(_initMethod, tt.Context, 0, RefKind.None, "ctx"));

            var body = Pchp.CodeAnalysis.CodeGen.MethodGenerator.GenerateMethodBody(module, _initMethod, builder, null, DiagnosticBag.GetInstance(), false);
            module.SetMethodBody(_initMethod, body);
        }
        SynthesizedMethodSymbol _initMethod;

        public SynthesizedStaticLocHolder(Source.SourceRoutineSymbol routine, string locName, TypeSymbol locType = null)
        {
            Contract.ThrowIfNull(routine);

            _routine = routine;
            _locName = locName ?? "?";
            _locType = locType;
        }

        public override int Arity => 0;

        internal override bool HasTypeArgumentsCustomModifiers => false;

        public override ImmutableArray<CustomModifier> GetTypeArgumentCustomModifiers(int ordinal) => GetEmptyTypeArgumentCustomModifiers(ordinal);

        public override Symbol ContainingSymbol => _routine.ContainingType;

        internal override PhpCompilation DeclaringCompilation => _routine.DeclaringCompilation;

        public override NamedTypeSymbol BaseType => DeclaringCompilation.CoreTypes.Object;

        public override Accessibility DeclaredAccessibility => Accessibility.Private;

        public override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override bool IsAbstract => false;

        public override bool IsSealed => true;

        public override bool IsStatic => false;

        public override bool IsSerializable => false;

        public override string Name => _routine.MetadataName.Replace('.', '-') + "$" + _locName;

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

        public override ImmutableArray<Symbol> GetMembersByPhpName(string name)
        {
            return GetMembers().Where(s => s.Name.StringsEqual(name, ignoreCase: true)).AsImmutable();
        }

        public override ImmutableArray<MethodSymbol> Constructors => ImmutableArray.Create((MethodSymbol)_ctor);

        public override ImmutableArray<MethodSymbol> InstanceConstructors => Constructors.Where(s => !s.IsStatic).AsImmutable();

        public override ImmutableArray<NamedTypeSymbol> GetTypeMembers() => ImmutableArray<NamedTypeSymbol>.Empty;

        public override ImmutableArray<NamedTypeSymbol> GetTypeMembers(string name) => ImmutableArray<NamedTypeSymbol>.Empty;

        internal override ImmutableArray<NamedTypeSymbol> GetDeclaredInterfaces(ConsList<Symbol> basesBeingResolved) => GetInterfacesToEmit();

        internal override IEnumerable<IFieldSymbol> GetFieldsToEmit()
        {
            return new IFieldSymbol[] { ValueField };
        }

        internal override ImmutableArray<NamedTypeSymbol> GetInterfacesToEmit()
        {
            if (_initMethod != null)
            {
                return ImmutableArray.Create(DeclaringCompilation.CoreTypes.IStaticInit.Symbol);
            }
            else
            {
                return ImmutableArray<NamedTypeSymbol>.Empty; 
            }
        }

        public override ImmutableArray<NamedTypeSymbol> AllInterfaces => GetInterfacesToEmit();
    }
}
