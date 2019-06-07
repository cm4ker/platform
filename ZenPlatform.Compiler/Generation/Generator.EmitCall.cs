using ZenPlatform.Compiler.AST.Definitions;
using ZenPlatform.Compiler.AST.Definitions.Expressions;
using ZenPlatform.Compiler.AST.Definitions.Functions;
using ZenPlatform.Compiler.AST.Definitions.Symbols;
using ZenPlatform.Compiler.AST.Infrastructure;
using ZenPlatform.Compiler.Contracts;

namespace ZenPlatform.Compiler.Generation
{
    public partial class Generator
    {
        private void EmitCall(IEmitter e, Call call, SymbolTable symbolTable)
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
                            else if (argument.Value is IndexerExpression ue)
                            {
                                Symbol variable = symbolTable.Find(((Name) ue.Value).Value, SymbolType.Variable);
                                if (variable.CodeObject is ILocal codeObject)
                                {
                                    if (((Variable) variable.SyntaxObject).Type.Type.IsArray)
                                        Error("ref cannot be applied to arrays");
                                    e.LdLocA(codeObject);
                                }
                                else if (variable.CodeObject is IField definition)
                                {
                                    if (((Variable) variable.SyntaxObject).Type.Type.IsArray)
                                        Error("ref cannot be applied to arrays");
                                    e.LdsFldA(definition);
                                }
                                else if (variable.CodeObject is IParameter pd)
                                {
                                    if (((Parameter) variable.SyntaxObject).Type.Type.IsArray)
                                        Error("ref cannot be applied to arrays");
                                    e.LdArgA(pd);
                                }

                                EmitExpression(e, ue.Indexer, symbolTable);
                                e.LdElemA();
                            }
                            else
                            {
                                Error("ref may only be applied to variables");
                            }
                        }
                        else
                        {
                            EmitExpression(e, argument.Value, symbolTable);
                        }
                    }
                }

                Hack:
                e.EmitCall((IMethod) symbol.CodeObject);
            }
            else
            {
                if (call.Name == "Read")
                {
//                    il.Emit(Mono.Cecil.Cil.OpCodes.Ldstr, "Input > ");
//                    MethodInfo write = System.Type.GetType("System.Console")
//                        .GetMethod("Write", new System.Type[] {typeof(string)});
//                    il.Emit(Mono.Cecil.Cil.OpCodes.Call, write, null);
//
//                    MethodInfo read = System.Type.GetType("System.Console").GetMethod("ReadLine");
//                    MethodInfo parse = System.Type.GetType("System.Int32")
//                        .GetMethod("Parse", new System.Type[] {typeof(string)});
//                    il.Emit(Mono.Cecil.Cil.OpCodes.Call, read, null);
//                    il.Emit(Mono.Cecil.Cil.OpCodes.Call, parse, null);
                }
                else if (call.Name == "Write")
                {
//                    EmitExpression(il, call.Arguments[0].Value, symbolTable);
//                    MethodInfo write = System.Type.GetType("System.Console")
//                        .GetMethod("WriteLine", new System.Type[] {typeof(int)});
//                    il.EmitCall(OpCodes.Call, write, null);
                }
                else
                {
                    Error("Unknown function name. [" + call.Name + "]");
                }
            }
        }
    }
}