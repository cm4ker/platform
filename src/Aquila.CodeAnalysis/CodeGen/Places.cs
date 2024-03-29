﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeGen;
using System;
using System.Diagnostics;
using System.Reflection.Metadata;
using Aquila.CodeAnalysis.Semantics;
using Aquila.CodeAnalysis.Symbols;
using Aquila.CodeAnalysis.Symbols.Source;
using Cci = Microsoft.Cci;
using FieldSymbol = Aquila.CodeAnalysis.Symbols.FieldSymbol;
using MethodSymbol = Aquila.CodeAnalysis.Symbols.MethodSymbol;
using ParameterSymbol = Aquila.CodeAnalysis.Symbols.ParameterSymbol;
using PropertySymbol = Aquila.CodeAnalysis.Symbols.PropertySymbol;
using SourceFieldSymbol = Aquila.CodeAnalysis.Symbols.SourceFieldSymbol;

namespace Aquila.CodeAnalysis.CodeGen
{
    #region IPlace

    /// <summary>
    /// Lightweight abstraction over a native storage supported by storage places with address.
    /// </summary>
    internal interface IPlace
    {
        /// <summary>
        /// Gets the type of place.
        /// </summary>
        TypeSymbol Type { get; }

        /// <summary>
        /// Emits code that loads the value from this storage place.
        /// </summary>
        /// <param name="il">The <see cref="ILBuilder"/> to emit the code to.</param>
        TypeSymbol EmitLoad(ILBuilder il);

        /// <summary>
        /// Emits preparation code for storing a value into the place.
        /// Must be call before loading a value and calling <see cref="EmitStore(ILBuilder)"/>.
        /// </summary>
        /// <param name="il">The <see cref="ILBuilder"/> to emit the code to.</param>
        void EmitStorePrepare(ILBuilder il);

        /// <summary>
        /// Emits code that stores a value to this storage place.
        /// </summary>
        /// <param name="il">The <see cref="ILBuilder"/> to emit the code to.</param>
        void EmitStore(ILBuilder il);

        /// <summary>
        /// Emits code that loads address of this storage place.
        /// </summary>
        /// <param name="il">The <see cref="ILBuilder"/> to emit the code to.</param>
        void EmitLoadAddress(ILBuilder il);

        /// <summary>
        /// Gets whether the place has an address.
        /// </summary>
        bool HasAddress { get; }
    }

    #endregion

    #region Places

    internal class LocalPlace : IPlace
    {
        readonly LocalDefinition _def;

        public override string ToString() => $"${_def.Name}";

        public LocalPlace(LocalDefinition def)
        {
            Contract.ThrowIfNull(def);
            _def = def;
        }

        public TypeSymbol Type => (TypeSymbol)_def.Type;

        public bool HasAddress => true;

        public TypeSymbol EmitLoad(ILBuilder il)
        {
            il.EmitLocalLoad(_def);
            return (TypeSymbol)_def.Type;
        }

        public void EmitLoadAddress(ILBuilder il) => il.EmitLocalAddress(_def);

        public void EmitStorePrepare(ILBuilder il)
        {
        }

        public void EmitStore(ILBuilder il) => il.EmitLocalStore(_def);
    }

    internal class ParamPlace : IPlace
    {
        readonly ParameterSymbol _p;

        public int Index => ((MethodSymbol)_p.ContainingSymbol).HasThis ? _p.Ordinal + 1 : _p.Ordinal;

        public override string ToString() => $"${_p.Name}";

        public ParamPlace(ParameterSymbol p)
        {
            Contract.ThrowIfNull(p);
            Debug.Assert(p.Ordinal >= 0, "(p.Ordinal < 0)");
            Debug.Assert(p is SourceParameterSymbol sp ? !sp.IsFake : true);
            _p = p;
        }

        public TypeSymbol Type => _p.Type;

        public bool HasAddress => true;

        public TypeSymbol EmitLoad(ILBuilder il)
        {
            il.EmitLoadArgumentOpcode(Index);
            return _p.Type;
        }

        public void EmitLoadAddress(ILBuilder il) => il.EmitLoadArgumentAddrOpcode(Index);

        public void EmitStorePrepare(ILBuilder il)
        {
        }

        public void EmitStore(ILBuilder il) => il.EmitStoreArgumentOpcode(Index);
    }

    internal class ArgPlace : IPlace
    {
        readonly int _index;
        readonly TypeSymbol _type;

        public int Index => _index;

        public override string ToString() => $"${_index}";

        public ArgPlace(TypeSymbol t, int index)
        {
            Contract.ThrowIfNull(t);
            _type = t;
            _index = index;
        }

        public TypeSymbol Type => _type;

        public bool HasAddress => true;

        public TypeSymbol EmitLoad(ILBuilder il)
        {
            il.EmitLoadArgumentOpcode(Index);
            return _type;
        }

        public void EmitLoadAddress(ILBuilder il) => il.EmitLoadArgumentAddrOpcode(Index);

        public void EmitStorePrepare(ILBuilder il)
        {
        }

        public void EmitStore(ILBuilder il) => il.EmitStoreArgumentOpcode(Index);
    }

    internal class ThisArgPlace : ArgPlace
    {
        public ThisArgPlace(TypeSymbol t) : base(t, 0)
        {
        }
    }

    /// <summary>
    /// Place wrapper allowing only read operation.
    /// </summary>
    internal class ReadOnlyPlace : IPlace
    {
        readonly IPlace _place;

        public ReadOnlyPlace(IPlace place)
        {
            Contract.ThrowIfNull(place);
            _place = place;
        }

        public bool HasAddress => _place.HasAddress;

        public TypeSymbol Type => _place.Type;

        public TypeSymbol EmitLoad(ILBuilder il) => _place.EmitLoad(il);

        public void EmitLoadAddress(ILBuilder il) => _place.EmitLoadAddress(il);

        public void EmitStore(ILBuilder il)
        {
            throw new InvalidOperationException($"{_place} is readonly!"); // TODO: ErrCode
        }

        public void EmitStorePrepare(ILBuilder il)
        {
        }
    }

    internal class FieldPlace : IPlace
    {
        readonly protected FieldSymbol _field;
        readonly protected Cci.IFieldReference _fieldref;
        readonly protected IPlace _holder;


        internal static IFieldSymbol GetRealDefinition(IFieldSymbol field)
        {
            // field redeclares its parent member, use the original def
            return (field is SourceFieldSymbol srcf)
                ? srcf.OverridenDefinition ?? field
                : field;
        }

        public FieldPlace(IFieldSymbol field, Emit.PEModuleBuilder module = null)
        {
            Contract.ThrowIfNull(field);

            field = GetRealDefinition(field);

            _field = (FieldSymbol)field;
            _fieldref = (module != null)
                ? module.Translate((FieldSymbol)field, null, DiagnosticBag.GetInstance())
                : (Cci.IFieldReference)field;
        }

        public FieldPlace(IFieldSymbol field, IPlace holder, Emit.PEModuleBuilder module = null) : this(field, module)
        {
            _holder = holder;
        }

        protected virtual void EmitHolder(ILBuilder il)
        {
            if (!_field.IsStatic)
                _holder?.EmitLoad(il);
        }

        void EmitOpCode(ILBuilder il, ILOpCode code)
        {
            il.EmitOpCode(code);
            il.EmitToken(_fieldref, null, DiagnosticBag.GetInstance());
        }

        public TypeSymbol Type => _field.Type;

        public bool HasAddress => true;

        public TypeSymbol EmitLoad(ILBuilder il)
        {
            EmitHolder(il);

            EmitOpCode(il, _field.IsStatic ? ILOpCode.Ldsfld : ILOpCode.Ldfld);
            return _field.Type;
        }

        public void EmitStorePrepare(ILBuilder il)
        {
            EmitHolder(il);
        }

        public void EmitStore(ILBuilder il)
        {
            EmitOpCode(il, _field.IsStatic ? ILOpCode.Stsfld : ILOpCode.Stfld);
        }

        public void EmitLoadAddress(ILBuilder il)
        {
            EmitHolder(il);
            EmitOpCode(il, _field.IsStatic ? ILOpCode.Ldsflda : ILOpCode.Ldflda);
        }
    }

    internal class PropertyPlace : IPlace
    {
        readonly IPlace _holder;
        readonly PropertySymbol _property;
        readonly Emit.PEModuleBuilder _module;

        public PropertyPlace(IPlace holder, Cci.IPropertyDefinition property, Emit.PEModuleBuilder module = null)
        {
            Contract.ThrowIfNull(property);

            _holder = holder;
            _property = (PropertySymbol)property;
            _module = module;
        }

        public TypeSymbol Type => _property.Type;

        public bool HasAddress => false;

        TypeSymbol EmitReceiver(ILBuilder il)
        {
            var lhs = VariableReferenceExtensions.EmitReceiver(il, _holder);

            if (_property.IsStatic)
            {
                if (lhs.Stack != null && !lhs.Stack.IsVoid())
                {
                    il.EmitOpCode(ILOpCode.Pop);
                }

                return null;
            }

            return lhs.Stack;
        }

        public TypeSymbol EmitLoad(ILBuilder il)
        {
            var stack = +1;
            var getter = _property.GetMethod;

            var receiver = EmitReceiver(il);
            if (receiver != null)
            {
                stack -= 1;
            }

            il.EmitOpCode((getter.IsVirtual || getter.IsAbstract) ? ILOpCode.Callvirt : ILOpCode.Call, stack);

            var getterref = (_module != null)
                ? _module.Translate(getter, DiagnosticBag.GetInstance(), false)
                : getter;

            il.EmitToken(getterref, null, DiagnosticBag.GetInstance()); // TODO: Translate

            //
            return getter.ReturnType;
        }

        public void EmitLoadAddress(ILBuilder il)
        {
            throw new NotSupportedException();
        }

        public void EmitStorePrepare(ILBuilder il)
        {
            EmitReceiver(il);
        }

        public void EmitStore(ILBuilder il)
        {
            var stack = 0;
            var setter = _property.SetMethod;

            if (_holder != null)
            {
                stack -= 1;
            }

            //
            il.EmitOpCode(setter.IsVirtual ? ILOpCode.Callvirt : ILOpCode.Call, stack);

            var setterref = (_module != null)
                ? _module.Translate(setter, DiagnosticBag.GetInstance(), false)
                : setter;

            il.EmitToken(setterref, null, DiagnosticBag.GetInstance()); // TODO: Translate

            //
            Debug.Assert(setter.ReturnType.SpecialType == SpecialType.System_Void);
        }
    }

    internal class OperatorPlace : IPlace
    {
        readonly MethodSymbol _operator;
        readonly IPlace _operand;

        public OperatorPlace(MethodSymbol @operator, IPlace operand)
        {
            Debug.Assert(@operator != null);
            Debug.Assert(@operator.HasThis == false);
            Debug.Assert(operand != null);

            _operator = @operator;
            _operand = operand;
        }

        public TypeSymbol Type => _operator.ReturnType;

        public bool HasAddress => false;

        public TypeSymbol EmitLoad(ILBuilder il)
        {
            _operand.EmitLoad(il);

            il.EmitOpCode(ILOpCode.Call, _operator.GetCallStackBehavior());
            il.EmitToken(_operator, null, DiagnosticBag.GetInstance());

            return Type;
        }

        public void EmitLoadAddress(ILBuilder il)
        {
            throw new NotSupportedException();
        }

        public void EmitStore(ILBuilder il)
        {
            throw new NotSupportedException();
        }

        public void EmitStorePrepare(ILBuilder il)
        {
            throw new NotSupportedException();
        }
    }

    #endregion
}