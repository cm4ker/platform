using System.Reflection.Emit;
using Mono.Cecil;
using Mono.Cecil.Cil;
using ZenPlatform.Language.AST.Definitions;
using ZenPlatform.Language.AST.Definitions.Expression;
using ZenPlatform.Language.AST.Definitions.Functions;
using ZenPlatform.Language.AST.Definitions.Symbols;
using ZenPlatform.Language.AST.Infrastructure;

namespace ZenPlatform.Language.Generation
{
    public partial class Generator
    {
        private void EmitCallStatement(ILProcessor il, CallStatement call, SymbolTable symbolTable)
        {
            Symbol symbol = symbolTable.Find(call.Name, SymbolType.Function);

            if (symbol != null)
            {
                Function function = symbol.SyntaxObject as Function;

                //
                // Check arguments
                //
                if (call.Arguments == null && function.Parameters == null)
                {
                    // Ugly hack.
                    goto Hack;
                }
                else if (call.Arguments.Count != function.Parameters.Count)
                {
                    Error("Argument mismatch [" + call.Name + "]");
                }
                else if (call.Arguments.Count != function.Parameters.Count)
                {
                    Error("Argument mismatch [" + call.Name + "]");
                }
                else
                {
                    for (int x = 0; x < call.Arguments.Count; x++)
                    {
                        if (call.Arguments[x].PassMethod != function.Parameters[x].PassMethod)
                        {
                            Error("Argument error [" + call.Name + "], argument [" + x + "] is wrong.");
                        }
                    }
                }

                if (call.Arguments != null)
                {
                    foreach (Argument argument in call.Arguments)
                    {
                        if (argument.PassMethod == PassMethod.ByReference)
                        {
                            // Regular value
                            if (argument.Value is Name)
                            {
                                Symbol variable = symbolTable.Find(((Name) argument.Value).Value, SymbolType.Variable);
                                if (variable.CodeObject is LocalBuilder)
                                {
                                    if (((Variable) variable.SyntaxObject).Type.VariableType ==
                                        VariableType.PrimitiveArray)
                                        Error("ref cannot be applied to arrays");
                                    il.Emit(Mono.Cecil.Cil.OpCodes.Ldloca, variable.CodeObject as VariableDefinition);
                                }
                                else if (variable.CodeObject is FieldBuilder)
                                {
                                    if (((Variable) variable.SyntaxObject).Type.VariableType ==
                                        VariableType.PrimitiveArray)
                                        Error("ref cannot be applied to arrays");
                                    il.Emit(Mono.Cecil.Cil.OpCodes.Ldsflda, variable.CodeObject as FieldDefinition);
                                }
                                else if (variable.CodeObject is ParameterBuilder)
                                {
                                    if (((Parameter) variable.SyntaxObject).Type.VariableType ==
                                        VariableType.PrimitiveArray)
                                        Error("ref cannot be applied to arrays");
                                    il.Emit(Mono.Cecil.Cil.OpCodes.Ldarga,
                                        ((ParameterBuilder) variable.CodeObject).Position - 1);
                                }
                            }
                            else if (argument.Value is UnaryExpression &&
                                     ((UnaryExpression) argument.Value).UnaryOperatorType == UnaryOperatorType.Indexer)
                            {
                                Symbol variable = symbolTable.Find(((Name) argument.Value).Value, SymbolType.Variable);
                                if (variable.CodeObject is VariableDefinition)
                                {
                                    il.Emit(Mono.Cecil.Cil.OpCodes.Ldloc, variable.CodeObject as VariableDefinition);
                                }
                                else if (variable.CodeObject is FieldBuilder)
                                {
                                    il.Emit(Mono.Cecil.Cil.OpCodes.Ldsfld, variable.CodeObject as FieldDefinition);
                                }
                                else if (variable.CodeObject is ParameterBuilder)
                                {
                                    il.Emit(Mono.Cecil.Cil.OpCodes.Ldarga,
                                        ((ParameterBuilder) variable.CodeObject).Position - 1);
                                }

                                EmitExpression(il, ((UnaryExpression) argument.Value).Indexer, symbolTable);
                                il.Emit(Mono.Cecil.Cil.OpCodes.Ldelema);
                            }
                            else
                            {
                                Error("ref may only be applied to variables");
                            }
                        }
                        else
                        {
                            EmitExpression(il, argument.Value, symbolTable);
                        }
                    }
                }

                Hack:
                il.Emit(Mono.Cecil.Cil.OpCodes.Call, ((MethodDefinition) symbol.CodeObject));
            }
            else
            {
//                if (call.Name == "Read")
//                {
//                    il.Emit(OpCodes.Ldstr, "Input > ");
//                    MethodInfo write = System.Type.GetType("System.Console")
//                        .GetMethod("Write", new System.Type[] {typeof(string)});
//                    il.EmitCall(OpCodes.Call, write, null);
//
//                    MethodInfo read = System.Type.GetType("System.Console").GetMethod("ReadLine");
//                    MethodInfo parse = System.Type.GetType("System.Int32")
//                        .GetMethod("Parse", new System.Type[] {typeof(string)});
//                    il.EmitCall(OpCodes.Call, read, null);
//                    il.EmitCall(OpCodes.Call, parse, null);
//                }
//                else if (call.Name == "Write")
//                {
//                    EmitExpression(il, call.Arguments[0].Value, symbolTable);
//                    MethodInfo write = System.Type.GetType("System.Console")
//                        .GetMethod("WriteLine", new System.Type[] {typeof(int)});
//                    il.EmitCall(OpCodes.Call, write, null);
//                }
//                else
//                {
//                    Error("Unknown function name. [" + call.Name + "]");
//                }
            }
        }
    }
}