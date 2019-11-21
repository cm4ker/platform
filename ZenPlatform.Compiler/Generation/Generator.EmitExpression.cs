using System;
using System.Globalization;
using System.Linq;
using ZenPlatform.Compiler.Contracts;
using ZenPlatform.Compiler.Contracts.Symbols;
using ZenPlatform.Compiler.Helpers;
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
                var variable = symbolTable.Find(name.Value, SymbolType.Variable, name.GetScope());

                if (variable == null)
                    Error("Assignment variable " + name.Value + " unknown.");

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

                if (expProp.PropertyType.IsArray)
                    fe.Type = new ArrayTypeSyntax(null,
                        new SingleTypeSyntax(null, expProp.PropertyType.ArrayElementType.Name, TypeNodeKind.Unknown));
                else
                    fe.Type = new SingleTypeSyntax(null, expProp.PropertyType.Name, TypeNodeKind.Unknown);

                if (expProp.Getter is null)
                    throw new Exception($"Can't resolve property: {fe.FieldName}");

                e.PropGetValue(expProp);
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
    }
}