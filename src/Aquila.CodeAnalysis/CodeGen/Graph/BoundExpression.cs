using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeGen;
using Aquila.CodeAnalysis.Symbols;
using Aquila.CodeAnalysis.Utilities;
using System;
using System.Diagnostics;
using System.Reflection.Metadata;
using Aquila.CodeAnalysis.CodeGen;
using Aquila.Syntax.Ast;
using Aquila.Syntax.Syntax;

namespace Aquila.CodeAnalysis.Semantics
{
    partial class BoundExpression
    {
        /// <summary>
        /// Emits the expression with its bound access.
        /// Only Read or None access is possible. Write access has to be handled separately.
        /// </summary>
        /// <param name="cg">Associated code generator.</param>
        /// <returns>The type of expression emitted on top of the evaluation stack.</returns>
        internal virtual TypeSymbol Emit(CodeGenerator cg)
        {
            throw ExceptionUtilities.UnexpectedValue(this.GetType().FullName);
        }
    }

    partial class BoundReferenceEx
    {
        /// <summary>
        /// Gets <see cref="IVariableReference"/> providing load and store operations.
        /// </summary>
        internal abstract IVariableReference BindPlace(CodeGenerator cg);

        internal abstract IPlace Place();

        internal override TypeSymbol Emit(CodeGenerator cg)
        {
            Debug.Assert(this.Access.IsRead || this.Access.IsNone);

            if (Access.IsNone)
            {
                // do nothing
                return cg.CoreTypes.Void;
            }

            if (ConstantValue.HasValue)
            {
                return cg.EmitLoadConstant(ConstantValue.Value, this.Access.TargetType);
            }

            var boundplace = this.BindPlace(cg);
            if (boundplace != null)
            {
                return boundplace.EmitLoadValue(cg, Access);
            }
            else
            {
                throw cg.NotImplementedException($"IVariableReference of {this} is null!");
            }
        }
    }

    partial class BoundListEx
    {
        internal override IVariableReference BindPlace(CodeGenerator cg)
        {
            throw new NotImplementedException();
        }

        internal override IPlace Place()
        {
            throw new NotImplementedException();
        }
    }

    partial class BoundArrayItemEx
    {
        internal override IVariableReference BindPlace(CodeGenerator cg)
        {
            throw new NotImplementedException();
        }

        internal override IPlace Place()
        {
            throw new NotImplementedException();
        }
    }

    partial class BoundVariableRef
    {
        internal override IVariableReference BindPlace(CodeGenerator cg) => this.Variable;

        internal override IPlace Place() => this.Variable.Place;
    }

    partial class BoundFieldRef
    {
        internal override IVariableReference BindPlace(CodeGenerator cg)
        {
            throw new NotImplementedException();
        }

        internal override IPlace Place()
        {
            throw new NotImplementedException();
        }
    }

    partial class BoundBinaryEx
    {
        internal override TypeSymbol Emit(CodeGenerator cg)
        {
            var il = cg.Builder;
            TypeSymbol res;

            res = cg.Emit(Left);
            cg.Emit(Right);

            switch (Operation)
            {
                case Operations.Add:
                    cg.EmitOpCode(ILOpCode.Add);
                    break;
                case Operations.Mul:
                    cg.EmitOpCode(ILOpCode.Mul);
                    break;
                case Operations.Div:
                    cg.EmitOpCode(ILOpCode.Div);
                    break;

                case Operations.GreaterThan:
                    cg.EmitOpCode(ILOpCode.Cgt);
                    res = cg.DeclaringCompilation.CoreTypes.Boolean;
                    break;
                case Operations.LessThan:
                    cg.EmitOpCode(ILOpCode.Clt);
                    res = cg.DeclaringCompilation.CoreTypes.Boolean;
                    break;


                default: throw new Exception("Op not supported");
            }


            return res;
        }
    }

    partial class BoundLiteral
    {
        internal override TypeSymbol Emit(CodeGenerator cg)
        {
            return cg.EmitLoadConstant(ConstantValue.Value, Access.TargetType);
        }
    }

    partial class BoundAssignEx
    {
        internal override TypeSymbol Emit(CodeGenerator cg)
        {
            var target_place = this.Target.BindPlace(cg);

            Debug.Assert(target_place != null);
            Debug.Assert(target_place.Type == null || target_place.Type.SpecialType != SpecialType.System_Void);

            // T tmp; // in case access is Read
            LocalDefinition tmp = null;

            // <target> = <value>
            var lhs = target_place.EmitStorePreamble(cg, Target.Access);

            var t_value = target_place.Type;
            if (t_value != null)
            {
                // we can convert more efficiently here
                cg.EmitConvert(Value, t_value);
            }
            else
            {
                t_value = cg.Emit(Value);
            }

            if (t_value.SpecialType == SpecialType.System_Void)
            {
                // default<T>
                t_value = target_place.Type;
                cg.EmitLoadDefault(t_value);
            }

            //
            if (Access.IsNone)
            {
                // nothing
            }
            else if (Access.IsRead)
            {
                tmp = cg.GetTemporaryLocal(t_value, false);
                cg.Builder.EmitOpCode(ILOpCode.Dup);
                cg.Builder.EmitLocalStore(tmp);
            }
            else
            {
                throw ExceptionUtilities.UnexpectedValue(Access);
            }

            target_place.EmitStore(cg, ref lhs, t_value, Target.Access);

            lhs.Dispose();

            //
            if (Access.IsNone)
            {
                t_value = cg.CoreTypes.Void;
            }
            else if (Access.IsRead)
            {
                Debug.Assert(tmp != null);
                cg.Builder.EmitLocalLoad(tmp);
            }

            if (tmp != null)
            {
                cg.ReturnTemporaryLocal(tmp);
            }

            //
            return t_value;
        }
    }

    partial class BoundIncDecEx
    {
        internal override TypeSymbol Emit(CodeGenerator cg)
        {
            Debug.Assert(this.Access.IsNone || Access.IsRead);
            Debug.Assert(!this.Access.IsReadRef);
            Debug.Assert(!this.Access.IsWrite);
            Debug.Assert(this.Target.Access.IsRead && this.Target.Access.IsWrite);
            Debug.Assert(this.Value.Access.IsRead);

            Debug.Assert(this.Value is BoundLiteral);

            TypeSymbol result_type = cg.CoreTypes.Void;
            LocalDefinition tempvar = null; // temporary variable containing result of the expression if needed

            var read = this.Access.IsRead;

            var target_place = this.Target.BindPlace(cg);
            Debug.Assert(target_place != null);

            // prepare target for store operation
            var lhs = target_place.EmitStorePreamble(cg, Target.Access);

            // load target value
            var target_load_type = target_place.EmitLoadValue(cg, ref lhs, Target.Access);

            TypeSymbol op_type = result_type;

            if (read && IsPostfix)
            {
                // store value of target
                // <temp> = TARGET
                tempvar = cg.GetTemporaryLocal(target_load_type);
                cg.EmitOpCode(ILOpCode.Dup);
                cg.Builder.EmitLocalStore(tempvar);
            }

            cg.Emit(Value);

            if (IsIncrement)
            {
                cg.EmitOpCode(ILOpCode.Add);
            }
            else
            {
                Debug.Assert(IsDecrement);
                cg.EmitOpCode(ILOpCode.Sub);
            }

            if (read && IsPrefix)
            {
                // store value of result
                // <temp> = TARGET
                tempvar = cg.GetTemporaryLocal(op_type);
                cg.EmitOpCode(ILOpCode.Dup);
                cg.Builder.EmitLocalStore(tempvar);
            }

            target_place.EmitStore(cg, ref lhs, op_type, Target.Access);

            lhs.Dispose();

            return op_type;
        }


        bool IsPrefix => !IsPostfix;
        bool IsDecrement => !this.IsIncrement;
    }


    partial class BoundStaticCallEx
    {
        internal override TypeSymbol Emit(CodeGenerator cg)
        {
            return cg.EmitCall(ILOpCode.Call, this.MethodSymbol, Instance, Arguments);
        }
    }

    partial class BoundInstanceCallEx
    {
        internal override TypeSymbol Emit(CodeGenerator cg)
        {
            return cg.EmitCall(ILOpCode.Call, this.MethodSymbol, Instance, Arguments);
        }
    }
}