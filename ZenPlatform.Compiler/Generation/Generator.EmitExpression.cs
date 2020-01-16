using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Mono.Cecil;
using ZenPlatform.Compiler.Contracts;
using ZenPlatform.Compiler.Contracts.Symbols;
using ZenPlatform.Compiler.Helpers;
using ZenPlatform.Core;
using ZenPlatform.Language.Ast;
using ZenPlatform.Language.Ast.Definitions;
using ZenPlatform.Language.Ast.Definitions.Expressions;
using ZenPlatform.Language.Ast.Definitions.Functions;
using ZenPlatform.Language.Ast.Infrastructure;
using ZenPlatform.Shared.Tree;

namespace ZenPlatform.Compiler.Generation
{
    public partial class Generator
    {
        private void EmitExpression(IEmitter e, Expression expression, SymbolTable symbolTable)
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
                            e.EmitCall(_bindings.Methods.Concat);
                            //e.EmitCall(_bindings.String.FindMethod(x => x.Name == "Concat" && x.Parameters.Count == 2));
                            break;
                        default: throw new NotSupportedException();
                    }

                if (be.Type.IsBoolean())
                {
                    switch (be.BinaryOperatorType)
                    {
                        case BinaryOperatorType.Equal:
                            e.Ceq();
                            break;
                        case BinaryOperatorType.NotEqual:
                            e.NotEqual();
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
                    e.LdElemI4();
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
                    EmitConvert(e, ce, symbolTable);
                }
            }
            else if (expression is Literal literal)
            {
                switch (literal.Type.Kind)
                {
                    case TypeNodeKind.Int:
                        e.LdcI4(Int32.Parse(literal.Value));
                        break;
                    case TypeNodeKind.String:
                        e.LdStr(literal.Value);
                        break;
                    case TypeNodeKind.Double:
                        e.LdcR8(double.Parse(literal.Value, CultureInfo.InvariantCulture));
                        break;
                    case TypeNodeKind.Char:
                        e.LdcI4(char.ConvertToUtf32(literal.Value, 0));
                        break;
                    case TypeNodeKind.Boolean:
                        e.LdcI4(bool.Parse(literal.Value) ? 1 : 0);
                        break;
                }
            }
            else if (expression is Name name)
            {
                var variable = symbolTable.Find(name.Value, SymbolType.Variable | SymbolType.Property, name.GetScope());

                if (variable == null)
                    Error("Variable " + name.Value + " are unknown.");

                if (variable.SyntaxObject is ContextVariable)
                {
                    CheckContextVariable(e, variable);
                }

                if (name.Type is null)
                    if (variable.SyntaxObject is ITypedNode tn)
                        name.Type = tn.Type;

                if (variable.CodeObject is ILocal vd)
                {
                    if (name.Type is UnionTypeSyntax)
                    {
                        e.LdLocA(vd);
                        e.EmitCall(_bindings.UnionTypeStorage.FindProperty("Value").Getter);
                    }
                    else
                        e.LdLoc(vd);
                }
                else if (variable.CodeObject is IField fd)
                {
                    e.LdArg_0();
                    e.LdFld(fd);
                }
                else if (variable.CodeObject is IParameter pd)
                {
                    Parameter p = variable.SyntaxObject as Parameter;

                    if (name.Type is UnionTypeSyntax)
                    {
                        e.LdArgA(pd);
                        e.EmitCall(_bindings.UnionTypeStorage.FindProperty("Value").Getter);
                    }
                    else
                        e.LdArg(pd.ArgIndex);

                    if (p.PassMethod == PassMethod.ByReference)
                        e.LdIndI4();
                }
                else if (variable.CodeObject is IProperty pr)
                {
                    throw new NotImplementedException();
                }
            }
            else if (expression is Call call)
            {
                EmitCall(e, call, symbolTable);
            }
            else if (expression is PropertyLookupExpression le)
            {
                EmitExpression(e, le.Current, symbolTable);

                var lna = le.Lookup as Name;

                var prop = _map.GetProperty(le.Current.Type, lna.Value);
                lna.Type = prop.PropertyType.ToAstType();

                e.EmitCall(prop.Getter);
            }

            else if (expression is MethodLookupExpression mle)
            {
                var lca = mle.Lookup as Call;

                EmitExpression(e, mle.Current, symbolTable);

                var method = _map.GetMethod(mle.Current.Type, lca.Name.Value,
                    lca.Arguments.Select(x => x.Expression.Type).ToArray());

                lca.Type = method.ReturnType.ToAstType();

                EmitArguments(e, lca.Arguments, symbolTable);
                e.EmitCall(method);
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
                var constructor = _ts.GetSystemBindings().Exception.FindConstructor(_bindings.String);
                e.NewObj(constructor);
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

        private void EmitPostOperation(IEmitter e, SymbolTable symbolTable, PostOperationExpression pis)
        {
            IType opType = pis.Type.ToClrType(_asm);

            if (pis.Expression is Name n)
            {
                var symbol = symbolTable.Find(n.Value, SymbolType.Variable | SymbolType.Property,
                    SymbolScopeBySecurity.Shared);

                var needLoc = symbol.Type == SymbolType.Property;

                ILocal loc = null;

                if (needLoc)
                    loc = e.DefineLocal(opType);

                EmitExpression(e, pis.Expression, symbolTable);

                if (needLoc)
                {
                    e.StLoc(loc)
                        .LdLoc(loc);
                }


                if (pis is PostIncrementExpression)
                    EmitIncrement(e, opType);
                else
                    EmitDecrement(e, opType);

                e.Add();

                if (symbol.CodeObject is IParameter pd)
                    e.StArg(pd);

                if (symbol.CodeObject is ILocal vd)
                    e.StLoc(vd);

                if (symbol.CodeObject is IProperty prd)
                {
                    e.LdArg_0()
                        .EmitCall(prd.Setter);
                }
            }
            else if (pis.Expression is PropertyLookupExpression ple)
            {
                var loc = e.DefineLocal(opType);
                
                //Load context
                EmitExpression(e, ple.Current, symbolTable);

                //Assign new value
                var prop = _map.GetProperty(ple.Current.Type, (ple.Lookup as Name).Value);
                e.LdLoc(loc);

                if (pis is PostIncrementExpression)
                    EmitIncrement(e, opType);
                else
                    EmitDecrement(e, opType);

                e.EmitCall(prop.Setter);
            }
        }


        private void CheckContextVariable(IEmitter e, ISymbol symbol)
        {
            if (symbol.CodeObject == null)
            {
                var loc = e.DefineLocal(_ts.FindType<PlatformContext>());

                e.LdContext()
                    .StLoc(loc);

                symbol.CodeObject = loc;
            }
        }
    }
}