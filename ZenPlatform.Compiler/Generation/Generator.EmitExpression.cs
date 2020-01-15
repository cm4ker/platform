using System;
using System.Globalization;
using System.Linq;
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
            else if (expression is GetFieldExpression fe)
            {
                EmitExpression(e, fe.Expression, symbolTable);
                var expType = fe.Expression.Type;

                IType extTypeScan = expType.ToClrType(_asm);

                var expProp = extTypeScan.Properties.First(x => x.Name == fe.FieldName);

                SingleTypeSyntax singleType;

                if (expProp.PropertyType.IsArray)
                {
                    singleType = new SingleTypeSyntax(null, expProp.PropertyType.ArrayElementType.FullName,
                        TypeNodeKind.Unknown);
                    fe.Type = new ArrayTypeSyntax(null, singleType);
                }
                else
                {
                    singleType = new SingleTypeSyntax(null, expProp.PropertyType.FullName, TypeNodeKind.Unknown);
                    fe.Type = singleType;
                }

                var resolved = singleType.ToClrType(_asm);

                if (resolved.IsPrimitive)
                {
                    singleType.ChangeKind(resolved.Name switch
                    {
                        "String" => TypeNodeKind.String,
                        "Int32" => TypeNodeKind.Int,
                        "Byte" => TypeNodeKind.Byte,
                        "Boolean" => TypeNodeKind.Boolean,
                        _ => throw new Exception($"New unknown primitive type {resolved.Name}")
                    });
                }
                else
                {
                    singleType.ChangeKind(TypeNodeKind.Object);
                }


                if (expProp.Getter is null)
                    throw new Exception($"Can't resolve property: {fe.FieldName}");

                e.PropGetValue(expProp);
            }
            else if (expression is LookupExpression le)
            {
                EmitExpression(e, le.Parent, symbolTable);

                if (le.Lookup is Name lna)
                {
                    var prop = _map.GetProperty(le.Parent.Type, lna.Value);
                    lna.Type = prop.PropertyType.ToAstType();

                    e.EmitCall(prop.Getter);
                }
                else if (le.Lookup is Call lca)
                {
                    var method = _map.GetMethod(le.Parent.Type, lca.Name,
                        lca.Arguments.Select(x => x.Expression.Type).ToArray());

                    lca.Type = method.ReturnType.ToAstType();

                    EmitArguments(e, lca.Arguments, symbolTable);
                    e.EmitCall(method);
                }
            }
            else if (expression is PostIncrementExpression pis)
            {
                var symbol = symbolTable.Find(pis.Name.Value, SymbolType.Variable, pis.GetScope()) ??
                             throw new Exception($"Variable {pis.Name} not found");

                IType opType = pis.Type.ToClrType(_asm);

                EmitExpression(e, pis.Name, symbolTable);

                EmitIncrement(e, opType);
                e.Add();

                if (symbol.CodeObject is IParameter pd)
                    e.StArg(pd);

                if (symbol.CodeObject is ILocal vd)
                    e.StLoc(vd);
            }
            else if (expression is PostDecrementExpression pds)
            {
                var symbol = symbolTable.Find(pds.Name.Value, SymbolType.Variable, pds.GetScope()) ??
                             throw new Exception($"Variable {pds.Name} not found");

                IType opType = null;
//                if (symbol.SyntaxObject is Parameter p)
//                    opType = p.Type.Type;
//                else if (symbol.SyntaxObject is Variable v)
//                    opType = v.Type.Type;


                EmitExpression(e, pds.Name, symbolTable);

                EmitDecrement(e, opType);
                e.Add();

                if (symbol.CodeObject is IParameter pd)
                    e.StArg(pd);

                if (symbol.CodeObject is ILocal vd)
                    e.StLoc(vd);
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