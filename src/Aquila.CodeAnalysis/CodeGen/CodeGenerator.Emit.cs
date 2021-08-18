using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeGen;
using Aquila.CodeAnalysis.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata;
using Aquila.CodeAnalysis.Emit;
using Aquila.CodeAnalysis.Semantics;
using Aquila.CodeAnalysis.Symbols;
using Aquila.Syntax;
using Aquila.Syntax.Text;


namespace Aquila.CodeAnalysis.CodeGen
{
    partial class CodeGenerator
    {
        /// <summary>
        /// Gets value indicating the method has locals already inicialized. 
        /// </summary>
        public bool InitializedLocals => _localsInitialized;

        public void EmitOpCode(ILOpCode code) => _il.EmitOpCode(code);

        public void EmitPop(TypeSymbol type)
        {
            Contract.ThrowIfNull(type);

            if (type.SpecialType != SpecialType.System_Void)
            {
                _il.EmitOpCode(ILOpCode.Pop, -1);
            }
        }

        /// <summary>
        /// Emits call to given method.
        /// </summary>
        /// <param name="code">Call op code, Call, Callvirt, Calli.</param>
        /// <param name="method">Method reference.</param>
        /// <returns>Method return type.</returns>
        internal TypeSymbol EmitCall(ILOpCode code, MethodSymbol method)
        {
            return _il.EmitCall(_moduleBuilder, _diagnostics, code, method);
        }

        internal TypeSymbol EmitLoadArgument(BoundArgument arg)
        {
            //load parameter
            return arg.Value.Emit(this);
        }

        internal TypeSymbol EmitCall(ILOpCode code, MethodSymbol method, BoundExpression thisExpr,
            ImmutableArray<BoundArgument> arguments, BoundTypeRef staticType = null)
        {
            Contract.ThrowIfNull(method);

            // {this}
            var thisType = (code != ILOpCode.Newobj) ? LoadTargetInstance(thisExpr, method) : null;

            // .callvirt -> .call
            if (code == ILOpCode.Callvirt && (!method.HasThis || !method.IsMetadataVirtual()))
            {
                // ignores null check in method call
                code = ILOpCode.Call;
            }

            // arguments
            foreach (var arg in arguments)
            {
                EmitLoadArgument(arg);
            }

            // call the method
            var result = EmitCall(code, method);

            //
            return result;
        }

        /// <summary>
        /// Emits <paramref name="thisExpr"/> to be used as target instance of method call, field or property.
        /// </summary>
        internal TypeSymbol LoadTargetInstance(BoundExpression thisExpr, MethodSymbol method)
        {
            NamedTypeSymbol targetType = method.HasThis ? method.ContainingType : CoreTypes.Void;

            if (thisExpr != null)
            {
                thisExpr.Emit(this);
            }
            else
            {
            }

            return method.ContainingType;
        }


        internal void EmitSymbolToken(TypeSymbol symbol, SyntaxNode syntaxNode)
        {
            _il.EmitSymbolToken(_moduleBuilder, _diagnostics, symbol, syntaxNode);
        }

        internal void EmitSymbolToken(FieldSymbol symbol, SyntaxNode syntaxNode)
        {
            _il.EmitSymbolToken(_moduleBuilder, _diagnostics, symbol, syntaxNode);
        }

        internal void EmitSymbolToken(MethodSymbol method, SyntaxNode syntaxNode)
        {
            _il.EmitSymbolToken(_moduleBuilder, _diagnostics, method, syntaxNode);
        }

        /// <summary>
        /// Emits <c>default(valuetype)</c>.
        /// Handles special types with a default ctor.
        /// </summary>
        public TypeSymbol EmitLoadDefaultOfValueType(TypeSymbol valuetype)
        {
            Debug.Assert(valuetype != null && valuetype.IsValueType);

            //TODO: Implement;

            throw new NotImplementedException();

            return valuetype;
        }

        /// <summary>
        /// Loads <see cref="RuntimeTypeHandle"/> of given type.
        /// </summary>
        public TypeSymbol EmitLoadToken(TypeSymbol type, SyntaxNode syntaxNodeOpt)
        {
            if (type.IsValidType())
            {
                _il.EmitLoadToken(_moduleBuilder, _diagnostics, type, syntaxNodeOpt);
            }
            else
            {
                EmitLoadDefaultOfValueType(this.CoreTypes.RuntimeTypeHandle);
            }

            return this.CoreTypes.RuntimeTypeHandle;
        }

        /// <summary>
        /// Emits <c>typeof(symbol) : System.Type</c>.
        /// </summary>
        internal TypeSymbol EmitSystemType(TypeSymbol symbol)
        {
            // ldtoken !!T
            EmitLoadToken(symbol, null);

            // call class System.Type System.Type::GetTypeFromHandle(valuetype System.RuntimeTypeHandle)
            return EmitCall(ILOpCode.Call,
                (MethodSymbol)DeclaringCompilation.GetWellKnownTypeMember(WellKnownMember
                    .System_Type__GetTypeFromHandle));
        }

        internal void EmitSequencePoint(Span span)
        {
            if (EmitPdbSequencePoints && span.IsValid && !span.IsEmpty)
            {
                EmitSequencePoint(span.ToTextSpan());
            }
        }

        internal void EmitSequencePoint(Microsoft.CodeAnalysis.Text.TextSpan span)
        {
            if (EmitPdbSequencePoints && span.Length > 0)
            {
                _il.EmitOpCode(ILOpCode.Nop);
                _il.DefineSequencePoint(_method.Syntax.SyntaxTree, span);
            }
        }

        internal void EmitSequencePoint(LangElement element)
        {
            if (element != null)
                EmitSequencePoint(element.Span);
        }

        internal void EmitHiddenSequencePoint()
        {
            if (EmitPdbSequencePoints)
            {
                _il.DefineHiddenSequencePoint();
            }
        }

        public TypeSymbol Emit(BoundExpression expr)
        {
            Contract.ThrowIfNull(expr);

            var t = expr.Emit(this);
            if (t == null)
            {
                throw ExceptionUtilities.UnexpectedValue(null);
            }

            return t;
        }

        /// <summary>
        /// Initializes place with a default value.
        /// This applies to structs without default ctor that won't work properly when uninitialized.
        /// </summary>
        internal void EmitInitializePlace(IPlace place)
        {
            Contract.ThrowIfNull(place);
            var t = place.Type;
            Contract.ThrowIfNull(t);

            switch (t.SpecialType)
            {
                // we don't have to initialize those:
                case SpecialType.System_Boolean:
                case SpecialType.System_Int32:
                case SpecialType.System_Int64:
                case SpecialType.System_Double:
                case SpecialType.System_Object:
                    break;

                default:
                    throw new NotImplementedException();
            }
        }


        public TypeSymbol EmitLoadConstant(object value, TypeSymbol targetOpt = null, bool notNull = false)
        {
            if (value == null)
            {
                if (notNull)
                {
                    // should be reported already
                    // Diagnostics.Add( ... )
                    Debug.Fail("value cannot be null");
                }

                if (targetOpt != null && targetOpt.IsValueType)
                {
                    // need to load default value type
                    throw new NotImplementedException();
                }
                else // reference type
                {
                    Builder.EmitNullConstant();
                    return targetOpt ?? CoreTypes.Object;
                }
            }
            else if (value is int i)
            {
                switch (targetOpt.GetSpecialTypeSafe())
                {
                    case SpecialType.System_Boolean:
                        _il.EmitBoolConstant(i != 0);
                        return targetOpt;
                    case SpecialType.System_Int64:
                        _il.EmitLongConstant(i);
                        return targetOpt;
                    case SpecialType.System_Double:
                        _il.EmitDoubleConstant(i);
                        return targetOpt;
                    case SpecialType.System_String:
                        _il.EmitStringConstant(i.ToString());
                        return targetOpt;
                }

                Builder.EmitIntConstant((int)value);
                return CoreTypes.Int32;
            }
            else if (value is long l)
            {
                switch (targetOpt.GetSpecialTypeSafe())
                {
                    case SpecialType.System_Boolean:
                        _il.EmitBoolConstant(l != 0);
                        return targetOpt;
                    case SpecialType.System_Int32:
                        _il.EmitIntConstant((int)l);
                        return targetOpt;
                    case SpecialType.System_Double:
                        _il.EmitDoubleConstant(l);
                        return targetOpt;
                    case SpecialType.System_Single:
                        _il.EmitSingleConstant(l);
                        return targetOpt;
                    case SpecialType.System_String:
                        _il.EmitStringConstant(l.ToString());
                        return targetOpt;
                    default:
                        break;
                }

                Builder.EmitLongConstant(l);
                return CoreTypes.Int64;
            }
            else if (value is string str)
            {
                switch (targetOpt.GetSpecialTypeSafe())
                {
                    case SpecialType.System_Char:
                        if (str != null && str.Length == 1)
                        {
                            Builder.EmitCharConstant(str[0]);
                            return targetOpt;
                        }

                        break;
                    case SpecialType.System_Int32:
                        if (int.TryParse(str, out i))
                        {
                            Builder.EmitIntConstant(i);
                            return targetOpt;
                        }

                        break;
                    case SpecialType.System_Int64:
                        if (long.TryParse(str, out l))
                        {
                            Builder.EmitLongConstant(l);
                            return targetOpt;
                        }

                        break;
                    case SpecialType.System_Double:
                        if (double.TryParse(str, out var d))
                        {
                            Builder.EmitDoubleConstant(d);
                            return targetOpt;
                        }

                        break;
                }

                Builder.EmitStringConstant(str);
                return CoreTypes.String;
            }
            else if (value is byte[] bytes)
            {
                throw new NotImplementedException();
            }
            else if (value is bool b)
            {
                switch (targetOpt.GetSpecialTypeSafe())
                {
                    case SpecialType.System_Boolean:
                        break;
                    case SpecialType.System_String:
                        _il.EmitStringConstant(b ? "1" : "");
                        return targetOpt;
                    default:
                        break;
                }

                // template: LOAD bool
                Builder.EmitBoolConstant(b);
                return CoreTypes.Boolean;
            }
            else if (value is double d)
            {
                switch (targetOpt.GetSpecialTypeSafe())
                {
                    case SpecialType.System_Boolean:
                        _il.EmitBoolConstant(d != 0.0);
                        return targetOpt;
                    case SpecialType.System_Int64:
                        _il.EmitLongConstant((long)d);
                        return targetOpt;
                }

                Builder.EmitDoubleConstant(d);
                return CoreTypes.Double;
            }
            else if (value is float)
            {
                Builder.EmitSingleConstant((float)value);
                return DeclaringCompilation.GetSpecialType(SpecialType.System_Single);
            }
            else if (value is uint)
            {
                Builder.EmitIntConstant(unchecked((int)(uint)value));
                return DeclaringCompilation.GetSpecialType(SpecialType.System_UInt32);
            }
            else if (value is ulong ul)
            {
                switch (targetOpt.GetSpecialTypeSafe())
                {
                    case SpecialType.System_Boolean:
                        _il.EmitBoolConstant(ul != 0.0);
                        return targetOpt;
                    case SpecialType.System_Int64:
                        _il.EmitLongConstant((long)ul);
                        return targetOpt;
                    case SpecialType.System_Double:
                        _il.EmitDoubleConstant((double)ul);
                        return targetOpt;
                    case SpecialType.System_String:
                        _il.EmitStringConstant(ul.ToString());
                        return targetOpt;
                }

                _il.EmitLongConstant(unchecked((long)ul));
                return DeclaringCompilation.GetSpecialType(SpecialType.System_UInt64);
            }
            else if (value is char)
            {
                switch (targetOpt.GetSpecialTypeSafe())
                {
                    case SpecialType.System_String:
                        Builder.EmitStringConstant(value.ToString());
                        return targetOpt;
                }

                Builder.EmitCharConstant((char)value);
                return DeclaringCompilation.GetSpecialType(SpecialType.System_Char);
            }
            else
            {
                throw ExceptionUtilities.UnexpectedValue(value);
            }
        }

        public TypeSymbol EmitLoadDefault(TypeSymbol type)
        {
            Debug.Assert(type != null);

            switch (type.SpecialType)
            {
                case SpecialType.System_Void:
                    break;
                case SpecialType.System_Double:
                    _il.EmitDoubleConstant(0.0);
                    break;
                case SpecialType.System_Int32:
                    _il.EmitIntConstant(0);
                    break;
                case SpecialType.System_Int64:
                    _il.EmitLongConstant(0);
                    break;
                case SpecialType.System_Boolean:
                    _il.EmitBoolConstant(false);
                    break;
                case SpecialType.System_Char:
                    _il.EmitCharConstant('\0');
                    break;
            }

            return type;
        }

        /// <summary>
        /// Emits .ret instruction with sequence point at closing brace.
        /// Eventually emits branching to closing block.
        /// </summary>
        public void EmitRet(TypeSymbol stack, bool yielding = false)
        {
            // sequence point
            var body = AstUtils.BodySpanOrInvalid(Method?.Syntax);
            if (body.IsValid && EmitPdbSequencePoints)
            {
                EmitSequencePoint(new Span(body.End - 1, 1));
            }

            //
            if (_il.InExceptionHandler || (ExtraFinallyBlock != null && !yielding))
            {
                this.ExitBlock.EmitTmpRet(this, stack, yielding);
            }
            else
            {
                _il.EmitRet(stack.SpecialType == SpecialType.System_Void);
            }
        }
    }


    internal static class ILBuilderExtension
    {
        public static void EmitLoadToken(this ILBuilder il, PEModuleBuilder module, DiagnosticBag diagnostics,
            TypeSymbol type, SyntaxNode syntaxNodeOpt)
        {
            il.EmitOpCode(ILOpCode.Ldtoken);
            EmitSymbolToken(il, module, diagnostics, type, syntaxNodeOpt);
        }

        public static void EmitLoadToken(this ILBuilder il, PEModuleBuilder module, DiagnosticBag diagnostics,
            MethodSymbol method, SyntaxNode syntaxNodeOpt)
        {
            il.EmitOpCode(ILOpCode.Ldtoken);
            EmitSymbolToken(il, module, diagnostics, method, syntaxNodeOpt);
        }

        public static void EmitSymbolToken(this ILBuilder il, PEModuleBuilder module, DiagnosticBag diagnostics,
            TypeSymbol symbol, SyntaxNode syntaxNode)
        {
            il.EmitToken(module.Translate(symbol, syntaxNode, diagnostics), syntaxNode, diagnostics);
        }

        public static void EmitSymbolToken(this ILBuilder il, PEModuleBuilder module, DiagnosticBag diagnostics,
            MethodSymbol symbol, SyntaxNode syntaxNode)
        {
            il.EmitToken(module.Translate(symbol, syntaxNode, diagnostics, needDeclaration: false), syntaxNode,
                diagnostics);
        }

        public static void EmitSymbolToken(this ILBuilder il, PEModuleBuilder module, DiagnosticBag diagnostics,
            FieldSymbol symbol, SyntaxNode syntaxNode)
        {
            il.EmitToken(module.Translate(symbol, syntaxNode, diagnostics), syntaxNode, diagnostics);
        }

        public static void EmitValueDefault(this ILBuilder il, PEModuleBuilder module, DiagnosticBag diagnostics,
            LocalDefinition tmp)
        {
            Debug.Assert(tmp.Type.IsValueType);
            il.EmitLocalAddress(tmp);
            il.EmitOpCode(ILOpCode.Initobj);
            il.EmitSymbolToken(module, diagnostics, (TypeSymbol)tmp.Type, null);
            // ldloc <loc>
            il.EmitLocalLoad(tmp);
        }

        /// <summary>
        /// Gets addr of a default value. Used to call a method on default value.
        /// </summary>
        public static void EmitValueDefaultAddr(this ILBuilder il, PEModuleBuilder module, DiagnosticBag diagnostics,
            LocalDefinition tmp)
        {
            Debug.Assert(tmp.Type.IsValueType);
            il.EmitLocalAddress(tmp);
            il.EmitOpCode(ILOpCode.Initobj);
            il.EmitSymbolToken(module, diagnostics, (TypeSymbol)tmp.Type, null);
            // ldloca <loc>
            il.EmitLocalAddress(tmp);
        }

        /// <summary>
        /// Gets or create a local variable and returns it back to pool.
        /// </summary>
        public static LocalDefinition GetTemporaryLocalAndReturn(this ILBuilder il, TypeSymbol t)
        {
            var definition =
                il.LocalSlotManager.AllocateSlot((Microsoft.Cci.ITypeReference)t, LocalSlotConstraints.None);

            il.LocalSlotManager.FreeSlot(definition);

            return definition;
        }

        /// <summary>
        /// Copies a value type from the top of evaluation stack into a temporary variable and loads its address.
        /// </summary>
        public static void EmitStructAddr(this ILBuilder il, TypeSymbol t)
        {
            Debug.Assert(t.IsStructType());

            var tmp = GetTemporaryLocalAndReturn(il, t);
            il.EmitLocalStore(tmp);
            il.EmitLocalAddress(tmp);
        }

        /// <summary>
        /// Emits call to given method.
        /// </summary>
        /// <returns>Method return type.</returns>
        public static TypeSymbol EmitCall(this ILBuilder il, PEModuleBuilder module, DiagnosticBag diagnostics,
            ILOpCode code, MethodSymbol method)
        {
            Contract.ThrowIfNull(method);
            Debug.Assert(code == ILOpCode.Call || code == ILOpCode.Calli || code == ILOpCode.Callvirt ||
                         code == ILOpCode.Newobj);
            Debug.Assert(!method.IsErrorMethodOrNull());

            var stack = method.GetCallStackBehavior();

            if (code == ILOpCode.Newobj)
            {
                stack += 1 + 1; // there is no <this>, + it pushes <newinst> on stack
            }

            if (code == ILOpCode.Callvirt && !method.IsAbstract &&
                (!method.IsVirtual || method.IsSealed || method.ContainingType.IsSealed))
            {
                code = ILOpCode.Call; // virtual dispatch is unnecessary
            }

            il.EmitOpCode(code, stack);
            il.EmitToken(module.Translate(method, diagnostics, false), null, diagnostics);
            return (code == ILOpCode.Newobj) ? method.ContainingType : method.ReturnType;
        }

        public static void EmitCharConstant(this ILBuilder il, char value)
        {
            il.EmitIntConstant(unchecked((int)value));
        }

        public static TypeSymbol EmitLoad(this ParameterSymbol p, ILBuilder il)
        {
            Debug.Assert(p != null, nameof(p));

            var index = p.Ordinal;
            var hasthis = ((MethodSymbol)p.ContainingSymbol).HasThis ? 1 : 0;

            il.EmitLoadArgumentOpcode(index + hasthis);
            return p.Type;
        }

        public static TypeSymbol EmitLoad(this FieldSymbol f, CodeGenerator cg)
        {
            Debug.Assert(f != null, nameof(f));

            if (!f.IsStatic)
            {
            }

            // .ldfld/.ldsfld {f}
            cg.Builder.EmitOpCode(f.IsStatic ? ILOpCode.Ldsfld : ILOpCode.Ldfld);
            cg.Builder.EmitToken(cg.Module.Translate(f, null, DiagnosticBag.GetInstance()), null,
                DiagnosticBag.GetInstance());

            //
            return f.Type;
        }


        public static LocalDefinition DefineSynthLocal(this ILBuilder il, MethodSymbol method, string name,
            NamedTypeSymbol type)
        {
            var loc = new SynthesizedLocalSymbol(method, name, type);
            var locDef = il.LocalSlotManager.DeclareLocal(type, loc,
                loc.Name, loc.SynthesizedKind, LocalDebugId.None, LocalVariableAttributes.None,
                LocalSlotConstraints.None, ImmutableArray<bool>.Empty, ImmutableArray<string>.Empty, false);

            return locDef;
        }
    }
}