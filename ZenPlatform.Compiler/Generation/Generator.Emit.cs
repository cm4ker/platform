using System;
using System.Linq;
using Mono.CompilerServices.SymbolWriter;
using ZenPlatform.Compiler.Contracts;
using ZenPlatform.Compiler.Contracts.Symbols;
using ZenPlatform.Compiler.Helpers;
using ZenPlatform.Language.Ast.Definitions;
using ZenPlatform.Language.Ast.Definitions.Expressions;
using ZenPlatform.Language.Ast.Definitions.Functions;
using ZenPlatform.Core.Network;

namespace ZenPlatform.Compiler.Generation
{
    public partial class Generator
    {
        private void EmitFunction(Function function)
        {
            if (function == null)
                throw new ArgumentNullException();
            if (function.Flags == FunctionFlags.ServerClientCall)
            {
                EmitRemoteCall(function);
                return;
            }

            IEmitter emitter = function.Builder;
            emitter.InitLocals = true;

            ILocal resultVar = null;
            if (function.Type.Kind != TypeNodeKind.Void)
                resultVar = emitter.DefineLocal(function.Type.ToClrType(_asm));

            var returnLabel = emitter.DefineLabel();
            EmitBody(emitter, function.Block, returnLabel, ref resultVar);
            emitter.MarkLabel(returnLabel);

            if (resultVar != null)
                emitter.LdLoc(resultVar);

            emitter.Ret();
        }

        private void EmitConvert(IEmitter e, CastExpression expression, SymbolTable symbolTable)
        {
            if (expression.Expression is Name name)
            {
                var variable = symbolTable.Find(name.Value, SymbolType.Variable);
                if (variable == null)
                    Error("Assignment variable " + name.Value + " unknown.");

                if (variable.SyntaxObject is Variable v)
                    expression.Expression.Type = v.Type;
                else if (variable.SyntaxObject is Parameter p)
                    expression.Expression.Type = p.Type;
            }


            //TODO: Нужно доделать ComputingEngine
//            TypeNode valueType;
//
//            if (expression.Value is IndexerExpression ie && ie.Type is ArrayTypeNode atn)
//            {
//                valueType = atn.ElementType;
//            }
//
//
//            var convertType = expression.Type.Type;
//            if (valueType is null || (valueType.IsValueType && convertType.IsValueType))
//            {
//                EmitConvCode(e, convertType);
//            }
        }

        private void EmitConvCode(IEmitter e, IType type)
        {
            if (type.Equals(_bindings.Int))
                e.ConvI4();
            else if (type.Equals(_bindings.Double))
                e.ConvR8();
            else if (type.Equals(_bindings.Char))
                e.ConvU2();
            else
                throw new Exception("Converting to this value not supported");
        }

        private void EmitAddValue(IEmitter e, IType type, int value)
        {
            if (type.Equals(_bindings.Int))
                e.LdcI4(value);
            else if (type.Equals(_bindings.Double))
                e.LdcR8(value);
            else if (type.Equals(_bindings.Char))
                e.LdcI4(value);
        }
    }
}