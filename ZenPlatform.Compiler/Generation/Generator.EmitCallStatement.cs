using ZenPlatform.Compiler.AST.Definitions;
using ZenPlatform.Compiler.AST.Definitions.Symbols;
using ZenPlatform.Compiler.AST.Infrastructure;
using ZenPlatform.Compiler.Contracts;
using ZenPlatform.Compiler.Contracts.Symbols;
using ZenPlatform.Language.Ast.AST.Definitions;
using ZenPlatform.Language.Ast.AST.Definitions.Expressions;
using ZenPlatform.Language.Ast.AST.Definitions.Functions;
using ZenPlatform.Language.Ast.AST.Infrastructure;

namespace ZenPlatform.Compiler.Generation
{
    public partial class Generator
    {
        private void EmitCallStatement(IEmitter il, CallStatement call, SymbolTable symbolTable)
        {
            var symbol = symbolTable.Find(call.Name, SymbolType.Function);

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
                                var variable = symbolTable.Find(((Name) argument.Value).Value, SymbolType.Variable);
                                if (variable.CodeObject is ILocal definition)
                                {
                                    if (((Variable) variable.SyntaxObject).Type.Type.IsArray)
                                        Error("ref cannot be applied to arrays");
                                    il.LdLocA(definition);
                                }
                                else if (variable.CodeObject is IField variableCodeObject)
                                {
                                    if (((Variable) variable.SyntaxObject).Type.Type.IsArray)
                                        Error("ref cannot be applied to arrays");
                                    il.LdsFldA(variableCodeObject);
                                }
                                else if (variable.CodeObject is IParameter pb)
                                {
                                    if (((Parameter) variable.SyntaxObject).Type.Type.IsArray)
                                        Error("ref cannot be applied to arrays");
                                    il.LdArgA(pb);
                                }
                            }
                            else if (argument.Value is IndexerExpression ue)
                            {
                                var variable = symbolTable.Find(((Name) argument.Value).Value, SymbolType.Variable);
                                if (variable.CodeObject is ILocal vd)
                                {
                                    il.LdLoc(vd);
                                }
                                else if (variable.CodeObject is IField df)
                                {
                                    il.LdsFld(df);
                                }
                                else if (variable.CodeObject is IParameter pd)
                                {
                                    il.LdArgA(pd);
                                }

                                EmitExpression(il, ue.Indexer, symbolTable);
                                il.LdElemA();
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
                il.EmitCall((IMethod) symbol.CodeObject);
            }
        }
    }


    public partial class Generator
    {
    }
}