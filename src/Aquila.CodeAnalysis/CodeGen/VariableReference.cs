﻿using System;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Reflection.Metadata;
using Aquila.CodeAnalysis.CodeGen;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeGen;
using Aquila.CodeAnalysis.Symbols;
using Aquila.CodeAnalysis.Symbols.Source;
using Aquila.Syntax.Syntax;
using Aquila.CodeAnalysis.Utilities;
using Aquila.Syntax;
using Microsoft.CodeAnalysis.Symbols;
using Cci = Microsoft.Cci;


namespace Aquila.CodeAnalysis.Semantics
{
    #region LhsStack

    /// <summary>
    /// A helper maintaining what is loaded on stack when storing a chained variable.
    /// </summary>
    struct LhsStack : IDisposable
    {
        public static LhsStack operator +(LhsStack lhsreceiver, LhsStack lhsoverride)
        {
            return new LhsStack
            {
                Stack = lhsoverride.Stack ?? lhsreceiver.Stack,
                StackByRef = lhsoverride.Stack != null ? lhsoverride.StackByRef : lhsreceiver.StackByRef,
                CodeGenerator = lhsoverride.CodeGenerator ?? lhsreceiver.CodeGenerator,
            };
        }

        /// <summary>
        /// Loaded value on stack.
        /// </summary>
        public TypeSymbol Stack { get; set; }

        /// <summary>
        /// Loaded value on stack is address.
        /// </summary>
        public bool StackByRef { get; set; }

        /// <summary>
        /// Gets value whether to store receiver in temporary variable.
        /// </summary>
        public bool IsEnabled { get; set; }

        public CodeGenerator CodeGenerator { get; set; }

        //bool _lhsUsesStack; // when true, we can safely `.dup` instead of emitting preamble again

        LocalDefinition _receiverTemp; // receiver value had to be stored into temp, we can load it from there

        LocalDefinition _indexTemp; // name/index value had to be stored into temp, we can load it from there

        public TypeSymbol EmitReceiver(CodeGenerator cg, BoundExpression receiver)
        {
            Debug.Assert(receiver != null);

            if (_receiverTemp != null)
            {
                // <loc>
                cg.Builder.EmitLocalLoad(_receiverTemp);
            }
            else
            {
                receiver.Emit(cg);

                if (IsEnabled) // store the result
                {
                    Debug.Assert(CodeGenerator != null);

                    // (<loc> = <instance>);
                    _receiverTemp = cg.GetTemporaryLocal(receiver.ResultType);
                    cg.EmitOpCode(ILOpCode.Dup);
                    cg.Builder.EmitLocalStore(_receiverTemp);
                }
            }

            //

            return receiver.ResultType;
        }

        public void Dispose()
        {
            if (_receiverTemp != null)
            {
                CodeGenerator.ReturnTemporaryLocal(_receiverTemp);
                _receiverTemp = null;
            }

            if (_indexTemp != null)
            {
                CodeGenerator.ReturnTemporaryLocal(_indexTemp);
                _indexTemp = null;
            }
        }
    }

    #endregion

    #region VariableReferenceExtensions

    internal static class VariableReferenceExtensions
    {
        public static TypeSymbol EmitLoadValue(this IPlace place, CodeGenerator cg, ref LhsStack lhs,
            BoundAccess access)
        {
            var type = place.Type;

            return place.EmitLoad(cg.Builder);
        }

        /// <summary>
        /// Emits preamble to an assignment.
        /// </summary>
        /// <param name="place">Target place to be assigned to.</param>
        /// <param name="cg">Ref to <see cref="CodeGenerator"/>.</param>
        /// <param name="access">The place's access.</param>
        /// <returns></returns>
        public static LhsStack EmitStorePreamble(this IPlace place, CodeGenerator cg, BoundAccess access)
        {
            var type = place.Type;

            place.EmitStorePrepare(cg.Builder); // TODO: LhsStack
            return default;
        }

        public static void EmitStore(this IPlace place, CodeGenerator cg, ref LhsStack lhs, TypeSymbol stack,
            BoundAccess access)
        {
            var type = place.Type;
            place.EmitStore(cg.Builder);
        }

        /// <summary>
        /// NOTICE: temporary API, will be replaced with operators.
        /// </summary>
        public static TypeSymbol EmitLoadValue(this IVariableReference /*!*/reference, CodeGenerator /*!*/cg,
            BoundAccess access)
        {
            Debug.Assert(reference != null);
            Debug.Assert(cg != null);

            var lhs = default(LhsStack);

            return reference.EmitLoadValue(cg, ref lhs, access);
        }

        public static TypeSymbol EmitLoadValue(CodeGenerator cg, MethodSymbol method, IPlace receiverOpt)
        {
            using (var lhs = EmitReceiver(cg, receiverOpt))
            {
                return cg.EmitCall(ILOpCode.Callvirt /*changed to .call by EmitCall if possible*/, method);
            }
        }

        public static TypeSymbol EmitLoadValue(this PropertySymbol property, CodeGenerator cg, IPlace receiver)
        {
            return EmitLoadValue(cg, property.GetMethod, receiver);
        }

        public static void EmitStore(this IVariableReference target, CodeGenerator cg, LocalDefinition local,
            BoundAccess access)
        {
            var lhs = target.EmitStorePreamble(cg, access);
            cg.Builder.EmitLocalLoad(local);
            target.EmitStore(cg, ref lhs, (TypeSymbol)local.Type, access);

            lhs.Dispose();
        }

        public static void EmitStore(this IVariableReference target, CodeGenerator cg, IPlace place, BoundAccess access)
        {
            Debug.Assert(access.IsWrite || access.IsWriteRef); // Write or WriteRef

            var lhs = target.EmitStorePreamble(cg, access);
            var type = place.EmitLoad(cg.Builder);
            target.EmitStore(cg, ref lhs, type, access);

            lhs.Dispose();
        }

        public static void EmitStore(this IVariableReference target, CodeGenerator cg, Func<TypeSymbol> valueLoader,
            BoundAccess access)
        {
            Debug.Assert(access.IsWrite || access.IsWriteRef); // Write or WriteRef

            var lhs = target.EmitStorePreamble(cg, access);
            var type = valueLoader();
            target.EmitStore(cg, ref lhs, type, access);

            lhs.Dispose();
        }

        public static LhsStack EmitReceiver(CodeGenerator cg, Symbol symbol, TypeSymbol receiver)
        {
            return default;
        }

        public static LhsStack EmitReceiver(CodeGenerator cg, ref LhsStack lhs, Symbol symbol, BoundExpression receiver)
        {
            // TODO: try load Receiver address directly if necessary, otherwise cg.EmitStructAddr
            // TODO: use LhsStack

            var receiverType = receiver != null ? lhs.EmitReceiver(cg, receiver) : null;

            return EmitReceiver(cg, symbol, receiverType);
        }

        public static LhsStack EmitReceiver(ILBuilder il, IPlace receiver)
        {
            if (receiver == null)
            {
                return default;
            }

            var type = receiver.Type;
            if (type.IsValueType)
            {
                if (receiver.HasAddress)
                {
                    receiver.EmitLoadAddress(il);
                }
                else
                {
                    receiver.EmitLoad(il);
                    il.EmitStructAddr(type);
                }

                return new LhsStack { Stack = type, StackByRef = true, };
            }
            else
            {
                receiver.EmitLoad(il);

                return new LhsStack { Stack = type, StackByRef = false, };
            }
        }

        public static LhsStack EmitReceiver(CodeGenerator cg, IPlace receiver) => EmitReceiver(cg.Builder, receiver);
    }

    #endregion

    #region IVariableReference

    /// <summary>
    /// An object specifying a reference to a variable, a field, a property, an array item (a value in general).
    /// Used by <see cref="BoundReferenceExpression"/>.
    /// </summary>
    interface IVariableReference
    {
        /// <summary>
        /// Optional.
        /// Gets the referenced symbol.
        /// </summary>
        Symbol Symbol { get; }

        /// <summary>
        /// Gets native type of the variable.
        /// </summary>
        TypeSymbol Type { get; }

        /// <summary>
        /// Gets value indicating the native value can be accessed by address (<c>ref</c>).
        /// </summary>
        bool HasAddress { get; }

        /// <summary>
        /// Optional. Gets <see cref="IPlace"/> referring to the variable.
        /// </summary>
        /// <remarks>May be initialized lazily before emit and not during the analysis phase yet.</remarks>
        IPlace Place { get; }

        /// <summary>
        /// Prepare store operation for given access.
        /// Returns information on what was loaded onto the stack (receiver).
        /// </summary>
        LhsStack EmitStorePreamble(CodeGenerator cg, BoundAccess access);

        /// <summary>
        /// Stores the value on stack into the variable.
        /// </summary>
        /// <param name="cg">Reference to <see cref="CodeGenerator"/>.</param>
        /// <param name="lhs">Receiver loaded on stack by previous call to <see cref="EmitStorePreamble"/>.</param>
        /// <param name="stack">Value loaded on stack to be stored into the variable.</param>
        /// <param name="access">Access information.</param>
        void EmitStore(CodeGenerator cg, ref LhsStack lhs, TypeSymbol stack, BoundAccess access);

        /// <summary>
        /// Loads value with given access.
        /// </summary>
        /// <param name="cg">Reference to <see cref="CodeGenerator"/>.</param>
        /// <param name="lhs">Receiver loaded on stack with previous call to <see cref="EmitStorePreamble"/>.</param>
        /// <param name="access">Access information.</param>
        /// <returns>Loaded value (by value).</returns>
        TypeSymbol EmitLoadValue(CodeGenerator cg, ref LhsStack lhs, BoundAccess access);

        /// <summary>
        /// Loads value with given access.
        /// </summary>
        /// <returns>Loaded value (by ref).</returns>
        TypeSymbol EmitLoadAddress(CodeGenerator cg, ref LhsStack lhs);
    }

    #endregion

    #region Locals (local variables)

    /// <summary>
    /// Base class for local variables, parameters, $this, static locals and temporary (synthesized) locals.
    /// </summary>
    [DebuggerDisplay("Variable: ${Name,nq} : {Type,nq}")]
    class LocalVariableReference : IVariableReference
    {
        /// <summary>Variable kind.</summary>
        public VariableKind VariableKind { get; }

        /// <summary>Name of the variable.</summary>
        public string Name => BoundName.NameValue.Value ?? this.Symbol?.Name;

        /// <summary>
        /// Whether the variable is regular local on stack.
        /// Otherwise the variable is loaded from special ".locals" array of variables.
        /// </summary>
        internal virtual bool IsOptimized =>
            Symbol != null &&
            Method.IsGlobalScope == false &&
            (Method.Flags & FlowAnalysis.MethodFlags.RequiresLocalsArray) == 0;

        public BoundVariableName BoundName { get; } // TODO: move to IVariableReference?

        public Symbol Symbol { get; protected set; }

        /// <summary>Containing method symbol. Cannot be <c>null</c>.</summary>
        internal SourceMethodSymbol Method { get; }

        public virtual TypeSymbol Type => Symbol.GetTypeOrReturnType();

        public virtual bool HasAddress => true;

        public virtual IPlace Place { get; protected set; }

        protected SynthesizedLocalKind SynthesizedLocalKind
        {
            get
            {
                if (Symbol is SynthesizedLocalSymbol)
                {
                    return SynthesizedLocalKind.EmitterTemp;
                }

                if (VariableKind == VariableKind.LocalTemporalVariable)
                {
                    return SynthesizedLocalKind.LoweringTemp;
                }

                return SynthesizedLocalKind.UserDefined;
            }
        }

        public LocalVariableReference(VariableKind kind, SourceMethodSymbol method, Symbol symbol,
            BoundVariableName name)
        {
            this.VariableKind = kind;
            this.Method = method ?? throw ExceptionUtilities.ArgumentNull(nameof(method));
            this.Symbol = symbol;
            this.BoundName = name ?? throw ExceptionUtilities.ArgumentNull(nameof(name));
        }

        /// <summary>
        /// Emits initialization of the variable if needed.
        /// Called from within <see cref="Graph.StartBlock"/>.
        /// </summary>
        public virtual void EmitInit(CodeGenerator cg)
        {
            if (VariableKind == VariableKind.LocalTemporalVariable && cg.Method != null &&
                (cg.Method.Flags & FlowAnalysis.MethodFlags.IsGenerator) == 0)
            {
                // continue,
                // create Place
            }
            else if (IsOptimized || cg.InitializedLocals)
            {
                // do nothing,
                // Place == null
                return;
            }

            Debug.Assert(Symbol != null);

            // declare variable in global scope
            var il = cg.Builder;
            var def = il.LocalSlotManager.DeclareLocal(
                (Cci.ITypeReference)Symbol.GetTypeOrReturnType(), Symbol as ILocalSymbolInternal,
                this.Name, this.SynthesizedLocalKind,
                LocalDebugId.None, 0, LocalSlotConstraints.None, ImmutableArray<bool>.Empty,
                ImmutableArray<string>.Empty, false);
            il.AddLocalToScope(def);

            this.Place = new LocalPlace(def);
        }

        TypeSymbol LoadVariablesArray(CodeGenerator cg)
        {
            throw new NotImplementedException();
        }

        public virtual LhsStack EmitStorePreamble(CodeGenerator cg, BoundAccess access)
        {
            return default;
        }

        public virtual void EmitStore(CodeGenerator cg, ref LhsStack lhs, TypeSymbol stack, BoundAccess access)
        {
            if (Place != null)
            {
                Place.EmitStore(cg, ref lhs, stack, access);
            }
        }

        public virtual TypeSymbol EmitLoadValue(CodeGenerator cg, ref LhsStack lhs, BoundAccess access)
        {
            var place = this.Place;
            if (place != null)
            {
                return place.EmitLoadValue(cg, ref lhs, access);
            }

            throw ExceptionUtilities.UnexpectedValue(this);
        }

        public virtual TypeSymbol EmitLoadAddress(CodeGenerator cg, ref LhsStack lhs)
        {
            Place.EmitLoadAddress(cg.Builder);
            return Place.Type;
        }

        /// <summary>
        /// Template: new IndirectLocal( LOCALS, NAME )
        /// </summary>
        internal TypeSymbol LoadIndirectLocal(CodeGenerator cg)
        {
            throw new NotImplementedException();
        }
    }

    class ParameterReference : LocalVariableReference
    {
        #region IParameterSource, IParameterTarget

        static void EmitTypeCheck(CodeGenerator cg, IPlace valueplace, SourceParameterSymbol srcparam)
        {
        }

        /// <summary>
        /// Describes the parameter source place.
        /// </summary>
        interface IParameterSource
        {
            void EmitTypeCheck(CodeGenerator cg, SourceParameterSymbol srcp);

            /// <summary>Inplace copies the parameter.</summary>
            void EmitPass(CodeGenerator cg);

            /// <summary>Loads copied parameter value.</summary>
            TypeSymbol EmitLoad(CodeGenerator cg);
        }

        /// <summary>
        /// Describes the local variable target slot.
        /// </summary>
        interface IParameterTarget
        {
            void StorePrepare(CodeGenerator cg);
            void Store(CodeGenerator cg, TypeSymbol valuetype);
        }

        /// <summary>
        /// Parameter or local is real CLR value on stack.
        /// </summary>
        sealed class DirectParameter : IParameterSource, IParameterTarget
        {
            readonly IPlace _place;
            readonly SourceParameterSymbol _param;

            public DirectParameter(IPlace place, SourceParameterSymbol param)
            {
                Debug.Assert(place != null);
                _place = place;
                _param = param;
            }

            /// <summary>Loads copied parameter value.</summary>
            public TypeSymbol EmitLoad(CodeGenerator cg)
            {
                throw new NotImplementedException();
            }

            public void EmitPass(CodeGenerator cg)
            {
            }

            public void EmitTypeCheck(CodeGenerator cg, SourceParameterSymbol srcp)
            {
                ParameterReference.EmitTypeCheck(cg, _place, srcp);
            }

            public void Store(CodeGenerator cg, TypeSymbol valuetype)
            {
            }

            public void StorePrepare(CodeGenerator cg)
            {
                _place.EmitStorePrepare(cg.Builder); // nop
            }
        }

        /// <summary>
        /// Parameter is fake and is stored in {varargs} array.
        /// </summary>
        sealed class IndirectParameterSource : IParameterSource
        {
            readonly IPlace _varargsplace;
            readonly int _index;
            bool _isparams => _p.IsParams;
            bool _byref => _p.Syntax.PassMethod == PassMethod.ByReference;

            readonly SourceParameterSymbol _p;

            public IndirectParameterSource(SourceParameterSymbol p, ParameterSymbol varargparam)
            {
                Debug.Assert(p.IsFake);
                Debug.Assert(varargparam.Type.IsSZArray());

                _p = p;
                _varargsplace = new ParamPlace(varargparam);
                _index = p.Ordinal - varargparam.Ordinal;
                Debug.Assert(_index >= 0);
            }

            public TypeSymbol EmitLoad(CodeGenerator cg)
            {
                throw new NotImplementedException();
            }

            public void EmitPass(CodeGenerator cg) => throw ExceptionUtilities.Unreachable;

            public void EmitTypeCheck(CodeGenerator cg, SourceParameterSymbol srcp)
            {
                // throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Local variables are unoptimized, parameter must be stored in {locals} array.
        /// </summary>
        sealed class IndirectLocalTarget : IParameterTarget
        {
            readonly string _localname;

            public IndirectLocalTarget(string localname)
            {
                _localname = localname;
            }

            public void StorePrepare(CodeGenerator cg)
            {
            }

            public void Store(CodeGenerator cg, TypeSymbol valuetype)
            {
            }
        }

        #endregion

        public ParameterSymbol Parameter => (ParameterSymbol)Symbol;

        public override TypeSymbol Type => Place != null ? Place.Type : base.Type;

        public ParameterReference(ParameterSymbol symbol, SourceMethodSymbol method)
            : base(VariableKind.Parameter, method, symbol, new BoundVariableName(symbol.Name))
        {
        }

        public override void EmitInit(CodeGenerator cg)
        {
            if (cg.InitializedLocals)
            {
                return;
            }

            Place = new ParamPlace(Parameter);
        }
    }

    class ThisVariableReference : LocalVariableReference
    {
        public ThisVariableReference(SourceMethodSymbol method)
            : base(VariableKind.ThisParameter, method, null, new BoundVariableName(VariableName.ThisVariableName))
        {
        }

        internal override bool IsOptimized => true;

        public override IPlace Place
        {
            get => _boundplace ?? throw new NotImplementedException();
            protected set => throw ExceptionUtilities.Unreachable;
        }

        IPlace _boundplace;

        public override bool HasAddress => false;

        public override TypeSymbol Type => Method.ContainingType;

        public override void EmitInit(CodeGenerator cg)
        {
        }

        public override TypeSymbol EmitLoadValue(CodeGenerator cg, ref LhsStack lhs, BoundAccess access)
        {
            cg.EmitOpCode(ILOpCode.Ldarg_0);
            return Type;
        }

        public override LhsStack EmitStorePreamble(CodeGenerator cg, BoundAccess access)
        {
            // should be handled in DiagnosticWalker before this happens
            throw ExceptionUtilities.Unreachable;
        }

        public override void EmitStore(CodeGenerator cg, ref LhsStack lhs, TypeSymbol stack, BoundAccess access)
        {
            throw ExceptionUtilities.Unreachable;
        }
    }

    #endregion
}