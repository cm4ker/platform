using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Mono.Cecil;
using ZenPlatform.Compiler.Contracts;
using ZenPlatform.Compiler.Contracts.Symbols;
using ZenPlatform.Compiler.Helpers;
using ZenPlatform.Compiler.Roslyn;
using ZenPlatform.Compiler.Roslyn.RoslynBackend;
using ZenPlatform.Core;
using ZenPlatform.Language.Ast;
using ZenPlatform.Language.Ast.Definitions;
using ZenPlatform.Language.Ast.Definitions.Expressions;
using ZenPlatform.Language.Ast.Definitions.Functions;
using ZenPlatform.Language.Ast.Definitions.Statements;
using ZenPlatform.Language.Ast.Infrastructure;
using ZenPlatform.Language.Ast.Symbols;
using ZenPlatform.Shared.Tree;
using BinaryExpression = ZenPlatform.Language.Ast.Definitions.Expressions.BinaryExpression;
using Call = ZenPlatform.Language.Ast.Definitions.Call;
using CastExpression = ZenPlatform.Language.Ast.Definitions.Expressions.CastExpression;
using Expression = ZenPlatform.Language.Ast.Definitions.Expression;
using Literal = ZenPlatform.Language.Ast.Definitions.Literal;
using UnaryExpression = ZenPlatform.Language.Ast.Definitions.Expressions.UnaryExpression;

namespace ZenPlatform.Compiler.Generation
{
    public partial class Generator
    {
        private void EmitExpression(RBlockBuilder e, Expression expression, SymbolTable symbolTable)
        {
            if (expression is BinaryExpression be)
            {
                EmitExpression(e, ((BinaryExpression) expression).Left, symbolTable);
                EmitExpression(e, ((BinaryExpression) expression).Right, symbolTable);

                if (be.Type.IsNumeric())
                    switch (be.BinaryOperatorType)
                    {
                        case BinaryOperatorType.Add:
                            e.Add();
                            break;
                        case BinaryOperatorType.Subtract:
                            e.Sub();
                            break;
                        case BinaryOperatorType.Multiply:
                            e.Mul();
                            break;
                        case BinaryOperatorType.Divide:
                            e.Div();
                            break;
                        case BinaryOperatorType.Modulo:
                            e.Rem();
                            break;
                    }

                if (be.Type.IsString())
                    switch (be.BinaryOperatorType)
                    {
                        case BinaryOperatorType.Add:
                            e.Add();
                            break;
                    }

                if (be.Type.IsBoolean())
                {
                    switch (be.BinaryOperatorType)
                    {
                        case BinaryOperatorType.Equal:
                            e.Ceq();
                            break;
                        case BinaryOperatorType.NotEqual:
                            e.Cneq();
                            break;
                        case BinaryOperatorType.GreaterThen:
                            e.Cgt();
                            break;
                        case BinaryOperatorType.LessThen:
                            e.Clt();
                            break;
                        case BinaryOperatorType.GraterOrEqualTo:
                            e.GreaterOrEqual();
                            break;
                        case BinaryOperatorType.LessOrEqualTo:
                            e.LessOrEqual();
                            break;
                        case BinaryOperatorType.And:
                            e.And();
                            break;
                        case BinaryOperatorType.Or:
                            e.Or();
                            break;
                    }
                }
            }
            else if (expression is UnaryExpression ue)
            {
                if (ue is IndexerExpression ie)
                {
                    EmitExpression(e, ie.Expression, symbolTable);
                    EmitExpression(e, ie.Indexer, symbolTable);
                    e.LdElem();
                }

                if (ue is LogicalOrArithmeticExpression lae)

                    switch (lae.OperaotrType)
                    {
                        case UnaryOperatorType.Indexer:

                            break;
                        case UnaryOperatorType.Negative:
                            EmitExpression(e, lae.Expression, symbolTable);
                            e.Neg();
                            break;
                        case UnaryOperatorType.Not:
                            EmitExpression(e, lae.Expression, symbolTable);
                            e.Not();
                            break;
                    }

                if (ue is CastExpression ce)
                {
                    EmitExpression(e, ce.Expression, symbolTable);
                    //EmitConvert(e, ce, symbolTable);
                }
            }
            else if (expression is Literal literal)
            {
                switch (literal.Type.Kind)
                {
                    case TypeNodeKind.Int:
                        e.LdLit(Int32.Parse(literal.Value));
                        break;
                    case TypeNodeKind.String:
                        e.LdLit(literal.Value);
                        break;
                    case TypeNodeKind.Double:
                        e.LdLit(double.Parse(literal.Value, CultureInfo.InvariantCulture));
                        break;
                    case TypeNodeKind.Char:
                        e.LdLit(char.Parse(literal.Value));
                        break;
                    case TypeNodeKind.Boolean:
                        e.LdLit(bool.Parse(literal.Value) ? 1 : 0);
                        break;
                }
            }
            else if (expression is Name name)
            {
                var symbol = symbolTable.Find(name.Value, SymbolType.Variable | SymbolType.Property, name.GetScope());

                if (symbol == null)
                    Error("Variable " + name.Value + " are unknown.");

                if (symbol is VariableSymbol variable)
                {
                    if (variable.SyntaxObject is ContextVariable)
                    {
                        CheckContextVariable(e, variable);
                    }

                    if (name.Type is null)
                        if (variable.SyntaxObject is ITypedNode tn)
                            name.Type = tn.Type;

                    if (variable.CompileObject is RLocal vd)
                    {
                        if (name.Type is PrimitiveTypeSyntax pts && (pts.IsBoolean() || pts.IsNumeric()) && false)
                        {
                            //TODO: need understand then we must load variable\arg by ref. While force disable this tree
                            // e.LdLocA(vd);
                        }
                        else
                            e.LdLoc(vd);
                    }
                    else if (variable.CompileObject is IField fd)
                    {
                        // e.LdArg_0();
                        // e.LdFld(fd);
                    }
                    else if (variable.CompileObject is RoslynParameter pd)
                    {
                        e.LdArg(pd);

                        // if (p.PassMethod == PassMethod.ByReference)
                        //     e.LdIndI4();
                    }
                }
                else if (symbol is PropertySymbol ps)
                {
                    throw new NotImplementedException();
                }
            }
            else if (expression is Call call)
            {
                EmitCall(e, call, symbolTable);
            }
            else if (expression is ClrInternalCall internalCall)
            {
                EmitArguments(e, internalCall.Arguments, symbolTable);
                // e.Call(internalCall.Method);
            }
            else if (expression is PropertyLookupExpression le)
            {
                EmitExpression(e, le.Current, symbolTable);

                var lna = le.Lookup as Name;

                var prop = _map.GetProperty(le.Current.Type, lna.Value);
                lna.Type = prop.PropertyType.ToAstType();
                e.LdProp(prop);
            }
            else if (expression is MethodLookupExpression mle)
            {
                var lca = mle.Lookup as Call;

                EmitExpression(e, mle.Current, symbolTable);

                var method = _map.GetMethod(mle.Current.Type, lca.Name.Value,
                    lca.Arguments.Select(x => _map.GetClrType(x.Expression.Type)).ToArray());

                lca.Type = method.ReturnType.ToAstType();

                EmitArguments(e, lca.Arguments, symbolTable);
                e.Call(method);
            }

            else if (expression is PostIncrementExpression pis)
            {
                EmitPostOperation(e, symbolTable, pis);
            }
            else if (expression is PostDecrementExpression pds)
            {
                EmitPostOperation(e, symbolTable, pds);
            }
            else if (expression is Variable variable)
            {
                EmitVariable(e, symbolTable, variable);
            }
            else if (expression is Throw th)
            {
                EmitExpression(e, th.Exception, symbolTable);

                if (th.Exception != null)
                {
                    var constructor = _ts.GetSystemBindings().Exception.FindConstructor(_bindings.String);
                    e.NewObj(constructor);
                }
                else
                {
                    var constructor = _ts.GetSystemBindings().Exception.FindConstructor();
                    e.NewObj(constructor);
                }

                e.Throw();
            }
            else if (expression is Assignment asg)
            {
                EmitAssignment(e, asg, symbolTable);
            }
            else if (expression is GlobalVar gv)
            {
                EmitGlobalVar(e, _varManager.Root, gv.Expression, symbolTable);

                /*
                 Глобальное адрессное пространство.
                 
                 
                 Необходимо для того, чтобы компоненты могли свободно добавлять свою функциональность
                 и её удобно было использовать в коде
                 
                 
                 для прмера, чтобы создать элекмент используется следующая команда
                
                           FieldLookup
                   FieldLookup    FieldLookup
                   V       V      V            
                 $.Entity.Invoice.Create();  
                   ^              ^         
                   ComponentName  Method          
                          ^       
                          Object                          
                 => 
                 
                 $.Entity.Invoice.MyOwnStaticMethod();
                 $.MyModule.Test();
                 
                 SomeManager.Create(Type : 10);
                 
                 var Invoice = 
                 
                 ld_manager
                 ldc.i4.5
                 Create(int typeId)
                 
                 GlobalMap
                    Fild Or Method Syntax, CodeObject 
                                  
                 В переменную $ необходимо для этого зарегистрировать Свойство "Entity"
                 У которого есть свойство "Create"                 
                                  
                 
                 
                 */
            }
        }

        private void EmitPostOperation(RBlockBuilder e, SymbolTable symbolTable, PostOperationExpression pis)
        {
            if (pis.Expression is Name)
            {
                EmitExpression(e, pis.Expression, symbolTable);
                EmitExpression(e, pis.Expression, symbolTable);
                e.LdLit(1).Add().Assign();
            }


            // IType opType = _map.GetClrType(pis.Type);
            //
            // if (pis.Expression is Name n)
            // {
            //     EmitExpression(e, pis.Expression, symbolTable);
            //
            //     if (pis is PostIncrementExpression)
            //         EmitIncrement(e, opType);
            //     else
            //         EmitDecrement(e, opType);
            //
            //     e.Add();
            //
            //     if (symbol is VariableSymbol vs)
            //     {
            //         if (vs.CompileObject is IParameter pd)
            //             e.StArg(pd);
            //
            //         if (vs.CompileObject is ILocal vd)
            //             e.StLoc(vd);
            //     }
            //     else if (symbol is PropertySymbol ps)
            //     {
            //         e.LdArg_0()
            //             .EmitCall(ps.ClrProperty.Setter);
            //     }
            // }
            // else if (pis.Expression is PropertyLookupExpression ple)
            // {
            //     var loc = e.DefineLocal(opType);
            //
            //     //Load context
            //     EmitExpression(e, ple.Current, symbolTable);
            //
            //     //Assign new value
            //     var prop = _map.GetProperty(ple.Current.Type, (ple.Lookup as Name).Value);
            //     e.LdLoc(loc);
            //
            //     if (pis is PostIncrementExpression)
            //         EmitIncrement(e, opType);
            //     else
            //         EmitDecrement(e, opType);
            //
            //     e.EmitCall(prop.Setter);
            // }
        }


        private void CheckContextVariable(RBlockBuilder e, VariableSymbol symbol)
        {
            if (symbol.CompileObject == null)
            {
                var loc = e.DefineLocal(_ts.Resolve<PlatformContext>());

                e.LdContext()
                    .StLoc(loc);
                e.Statement();

                symbol.Connect(loc);
            }
        }
    }
}