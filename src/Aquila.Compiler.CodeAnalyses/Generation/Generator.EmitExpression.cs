using System;
using System.Globalization;
using System.Linq;
using Aquila.Compiler.Contracts;
using Aquila.Compiler.Helpers;
using Aquila.Compiler.Roslyn;
using Aquila.Compiler.Roslyn.RoslynBackend;
using Aquila.Compiler.Visitor;
using Aquila.Core;
using Aquila.Language.Ast;
using Aquila.Language.Ast.Definitions;
using Aquila.Language.Ast.Definitions.Expressions;
using Aquila.Language.Ast.Infrastructure;
using Aquila.Language.Ast.Misc;
using Aquila.Language.Ast.Symbols;
using Aquila.ServerRuntime;
using Assignment = Aquila.Language.Ast.Assignment;
using BinaryExpression = Aquila.Language.Ast.BinaryExpression;
using Call = Aquila.Language.Ast.Call;
using CastExpression = Aquila.Language.Ast.CastExpression;
using ContextVariable = Aquila.Language.Ast.ContextVariable;
using Expression = Aquila.Language.Ast.Expression;
using GlobalVar = Aquila.Language.Ast.GlobalVar;
using IndexerExpression = Aquila.Language.Ast.IndexerExpression;
using Literal = Aquila.Language.Ast.Literal;
using LogicalOrArithmeticExpression = Aquila.Language.Ast.LogicalOrArithmeticExpression;
using Name = Aquila.Language.Ast.Name;
using New = Aquila.Language.Ast.New;
using PostDecrementExpression = Aquila.Language.Ast.PostDecrementExpression;
using PostIncrementExpression = Aquila.Language.Ast.PostIncrementExpression;
using PrimitiveTypeSyntax = Aquila.Language.Ast.PrimitiveTypeSyntax;
using PropertyLookupExpression = Aquila.Language.Ast.PropertyLookupExpression;
using TypeSyntax = Aquila.Language.Ast.TypeSyntax;
using UnaryExpression = Aquila.Language.Ast.UnaryExpression;
using Variable = Aquila.Language.Ast.Variable;

namespace Aquila.Compiler.Generation
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
                    EmitCast(e, ce, symbolTable);
                }
            }
            else if (expression is Literal literal)
            {
                if (literal.IsSqlLiteral)
                {
                    //need compile sql expression!
                    var result = QueryCompilerHelper.Compile(_conf.TypeManager, literal.Value);
                    e.LdLit(result.sql);
                }
                else
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

                var lna = le.Lookup as Name ?? throw new Exception("Lookup mush be Name node type");

                if (le.Current.Type.Kind == TypeNodeKind.Type &&
                    TypeFinder.FindSymbol(le.Current.Type, _root)?.Type == QueryCompilerHelper.DataReader)
                {
                    //ugly hack
                    //TODO: introduce redirected dynamic properties

                    //this is reader type redirect propNames to expr[expr]
                    e.LdLit(lna.Value);
                    e.LdElem();
                }
                else
                {
                    var prop = _map.GetProperty(le.Current.Type, lna.Value);
                    lna.Type = prop.PropertyType.ToAstType();
                    e.LdProp(prop);
                }
            }
            else if (expression is MethodLookupExpression mle)
            {
                var lca = mle.Lookup as Call;

                EmitExpression(e, mle.Current, symbolTable);

                var method = _map.GetMethod(mle.Current.Type, lca.Name.Value,
                    lca.Arguments.Select(x => _map.GetClrType(x.Expression.Type)).ToArray());

                var resultType = (TypeSyntax) method.astMethod.Type.Clone();

                lca.Type = resultType;
                lca.Attach(resultType);

                EmitArguments(e, lca.Arguments, symbolTable);
                e.Call(method.clrMethod);
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

            else if (expression is New wen)
            {
                EmitArguments(e, wen.Call.Arguments, symbolTable);

                var type = _map.GetClrType(wen.Type);

                var constr =
                    type.FindConstructor(wen.Call.Arguments.Select(x => _map.GetClrType(x.Expression.Type)).ToArray());

                e.NewObj(constr);
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

                symbol.Connect(loc);
            }
        }
    }
}