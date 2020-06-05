using System;
using Aquila.Compiler.Aqua.TypeSystem;
using Aquila.Compiler.Aqua.TypeSystem.Builders;
using Aquila.Core.Contracts;
using Aquila.Language.Ast.Definitions;
using Aquila.Language.Ast.Definitions.Functions;
using Aquila.Language.Ast.Symbols;
using CastExpression = Aquila.Language.Ast.Definitions.Expressions.CastExpression;

namespace Aquila.Compiler.Generation
{
    public partial class Generator
    {
        private void EmitFunction(Function function, PMethodBuilder method)
        {
            if (function == null)
                throw new ArgumentNullException();

            if (method == null)
                throw new ArgumentNullException();

            EmitFunction(function, method.Body);
        }

        private void EmitFunction(Function function, PCilBody body)
        {
            if (function == null)
                throw new ArgumentNullException();

            if (function.Flags == FunctionFlags.ServerClientCall && _mode == CompilationMode.Client)
            {
                EmitRemoteCall(function);
                return;
            }

            var emitter = body;
            // emitter.InitLocals = true;

            PLocal resultVar = null;

            if (function.Type.Kind != TypeNodeKind.Void)
                resultVar = emitter.DefineLocal(_map.GetClrType(function.Type));

            var returnLabel = emitter.DefineLabel();

            EmitBody(emitter, function.Block, returnLabel, ref resultVar);
            emitter.MarkLabel(returnLabel);

            if (resultVar != null)
                emitter.LdLoc(resultVar);

            emitter.Ret();
        }

        private void EmitCast(PCilBody e, CastExpression cast, SymbolTable symbolTable)
        {
            EmitExpression(e, cast.Expression, symbolTable);
            e.Cast(_map.GetClrType(cast.CastType));

            // if (cast.Expression is Name name)
            // {
            //     var variable = symbolTable.Find<VariableSymbol>(name.Value, name.GetScope());
            //
            //     if (variable == null)
            //         Error("Assignment variable " + name.Value + " unknown.");
            //
            //     if (variable.SyntaxObject is Variable v)
            //         cast.Expression.Type = v.Type;
            //
            //     else if (variable.SyntaxObject is Parameter p)
            //         cast.Expression.Type = p.Type;
            //
            //
            // }

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

        // private void EmitConvCode(IEmitter e, IType type)
        // {
        //     if (type.Equals(_bindings.Int))
        //         e.ConvI4();
        //     else if (type.Equals(_bindings.Double))
        //         e.ConvR8();
        //     else if (type.Equals(_bindings.Char))
        //         e.ConvU2();
        //     else
        //         throw new Exception("Converting to this value not supported");
        // }
        //
        // private void EmitAddValue(IEmitter e, IType type, int value)
        // {
        //     if (type.Equals(_bindings.Int))
        //         e.LdcI4(value);
        //     else if (type.Equals(_bindings.Double))
        //         e.LdcR8(value);
        //     else if (type.Equals(_bindings.Char))
        //         e.LdcI4(value);
        // }
    }
}