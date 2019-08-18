using ZenPlatform.Compiler.AST.Infrastructure;
using ZenPlatform.Compiler.Contracts;
using ZenPlatform.Compiler.Contracts.Symbols;
using ZenPlatform.Language.Ast.Definitions;
using ZenPlatform.Language.Ast.Definitions.Expressions;
using ZenPlatform.Language.Ast.Definitions.Functions;
using ZenPlatform.Language.Ast.Infrastructure;

namespace ZenPlatform.Compiler.Generation
{
    public partial class Generator
    {
        private void EmitCall(IEmitter e, Call call, SymbolTable symbolTable)
        {
            var symbol = symbolTable.Find(call.Name, SymbolType.Function, call.GetScope());

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
                            if (argument.Expression is Name arg)
                            {
                                var variable = symbolTable.Find(arg.Value, SymbolType.Variable, arg.GetScope());
                                if (variable.CodeObject is ILocal vd)
                                {
                                    e.LdLocA(vd);
                                }
                                else if (variable.CodeObject is IField fd)
                                {
                                    e.LdsFldA(fd);
                                }
                                else if (variable.CodeObject is IParameter pb)
                                {
                                    e.LdArgA(pb);
                                }
                            }
                            else if (argument.Expression is IndexerExpression ue)
                            {
                                if (ue.Expression is Name uen)
                                {
                                    var variable = symbolTable.Find(uen.Value, SymbolType.Variable, uen.GetScope());
//                                    
//                                    if (variable.CodeObject is ILocal codeObject)
//                                    {
//                                        if (((Variable) variable.SyntaxObject).Type.Type.IsArray)
//                                            Error("ref cannot be applied to arrays");
//                                        e.LdLocA(codeObject);
//                                    }
//                                    else if (variable.CodeObject is IField definition)
//                                    {
//                                        if (((Variable) variable.SyntaxObject).Type.Type.IsArray)
//                                            Error("ref cannot be applied to arrays");
//                                        e.LdsFldA(definition);
//                                    }
//                                    else if (variable.CodeObject is IParameter pd)
//                                    {
//                                        if (((Parameter) variable.SyntaxObject).Type.Type.IsArray)
//                                            Error("ref cannot be applied to arrays");
//                                        e.LdArgA(pd);
//                                    }

                                    EmitExpression(e, ue.Indexer, symbolTable);
                                    e.LdElemA();
                                }
                            }
                            else
                            {
                                Error("ref may only be applied to variables");
                            }
                        }
                        else
                        {
                            EmitExpression(e, argument.Expression, symbolTable);
                        }
                    }
                }

                Hack:
                e.EmitCall((IMethod) symbol.CodeObject);
            }
            else
            {
                Error("Unknown function name. [" + call.Name + "]");
            }
        }
    }
}