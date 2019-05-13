using Mono.Cecil;
using Mono.Cecil.Cil;
using ZenPlatform.Compiler.AST.Definitions;
using ZenPlatform.Compiler.AST.Definitions.Expressions;
using ZenPlatform.Compiler.AST.Definitions.Functions;
using ZenPlatform.Compiler.AST.Definitions.Symbols;
using ZenPlatform.Compiler.AST.Infrastructure;

namespace ZenPlatform.Compiler.Cecil.Backend.Resolver
{
    public partial class SyntaxAnalyser
    {
        private void EmitCallStatement(Emitter il, CallStatement call, SymbolTable symbolTable)
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

                if (call.Arguments.Count != function.Parameters.Count)
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
                                if (variable.CodeObject is VariableDefinition definition)
                                {
                                    if (((Variable) variable.SyntaxObject).Type.IsArray)
                                        Error("ref cannot be applied to arrays");
                                    il.LdLocA(definition);
                                }
                                else if (variable.CodeObject is FieldDefinition)
                                {
                                    if (((Variable) variable.SyntaxObject).Type.IsArray)
                                        Error("ref cannot be applied to arrays");
                                    il.LdsFldA(variable.CodeObject as FieldDefinition);
                                }
                                else if (variable.CodeObject is ParameterDefinition pb)
                                {
                                    if (((Parameter) variable.SyntaxObject).Type.IsArray)
                                        Error("ref cannot be applied to arrays");
                                    il.LdArgA(pb.Sequence - 1);
                                }
                            }
                            else if (argument.Value is IndexerExpression ue)
                            {
                                Symbol variable = symbolTable.Find(((Name) argument.Value).Value, SymbolType.Variable);
                                if (variable.CodeObject is VariableDefinition vd)
                                {
                                    il.LdLoc(vd);
                                }
                                else if (variable.CodeObject is FieldDefinition definition)
                                {
                                    il.LdsFld(definition);
                                }
                                else if (variable.CodeObject is ParameterDefinition parameterDefinition)
                                {
                                    il.LdArgA(parameterDefinition.Sequence - 1);
                                }

                                ResolveExpression(il, ue.Indexer, symbolTable);
                                il.LdElemA();
                            }
                            else
                            {
                                Error("ref may only be applied to variables");
                            }
                        }
                        else
                        {
                            ResolveExpression(il, argument.Value, symbolTable);
                        }
                    }
                }

                Hack:
                il.Call(((MethodDefinition) symbol.CodeObject));
            }
        }
    }
}