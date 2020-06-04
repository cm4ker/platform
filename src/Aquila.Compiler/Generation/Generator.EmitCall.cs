using System.Collections.Generic;
using System.Linq;
using Aquila.Compiler.Aqua.TypeSystem;
using Aquila.Compiler.Contracts;
using Aquila.Compiler.Contracts.Symbols;
using Aquila.Compiler.Roslyn;
using Aquila.Compiler.Roslyn.RoslynBackend;
using Aquila.Language.Ast.Definitions;
using Aquila.Language.Ast.Definitions.Expressions;
using Aquila.Language.Ast.Definitions.Functions;
using Aquila.Language.Ast.Infrastructure;
using Aquila.Language.Ast.Symbols;
using Call = Aquila.Language.Ast.Definitions.Call;

namespace Aquila.Compiler.Generation
{
    public partial class Generator
    {
        private void EmitCall(PCilBody e, Call call, SymbolTable symbolTable)
        {
            var symbol = symbolTable.Find<MethodSymbol>(call.Name.Value, call.GetScope());

            if (symbol != null)
            {
                var func = symbol.SelectOverload(call.Arguments.Select(x => _map.GetClrType(x.Expression.Type))
                    .ToArray());

                //we need load this arg
                if (!func.clrMethod.IsStatic)
                    e.LdArg(0);

                Function function = func.method;

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
                    EmitArguments(e, call.Arguments, symbolTable);
                }

                Hack:
                e.Call(func.clrMethod);
            }
            else
            {
                Error("Unknown function name. [" + call.Name.Value + "]");
            }
        }

        private void EmitArguments(PCilBody e, ArgumentList args, SymbolTable symbolTable)
        {
            foreach (Argument argument in args)
            {
                if (argument.PassMethod == PassMethod.ByReference)
                {
                    // Regular value
                    if (argument.Expression is Name arg)
                    {
                        var variable = symbolTable.Find<VariableSymbol>(arg.Value, arg.GetScope());
                        if (variable.CompileObject is PLocal vd)
                        {
                            e.LdLoc(vd);
                        }
                        else if (variable.CompileObject is IField fd)
                        {
                            // e.LdsFldA(fd);
                        }
                        else if (variable.CompileObject is PParameter pb)
                        {
                            e.LdArg(pb);
                        }
                    }
                    else if (argument.Expression is IndexerExpression ue)
                    {
                        if (ue.Expression is Name uen)
                        {
                            var variable = symbolTable.Find(uen.Value, SymbolType.Variable, uen.GetScope());

                            // if (variable.CodeObject is ILocal codeObject)
                            // {
                            //     if (((Variable) variable.SyntaxObject).Type.Type.IsArray)
                            //         Error("ref cannot be applied to arrays");
                            //     e.LdLocA(codeObject);
                            // }
                            // else if (variable.CodeObject is IField definition)
                            // {
                            //     if (((Variable) variable.SyntaxObject).Type.Type.IsArray)
                            //         Error("ref cannot be applied to arrays");
                            //     e.LdsFldA(definition);
                            // }
                            // else if (variable.CodeObject is IParameter pd)
                            // {
                            //     if (((Parameter) variable.SyntaxObject).Type.Type.IsArray)
                            //         Error("ref cannot be applied to arrays");
                            //     e.LdArgA(pd);
                            // }

                            EmitExpression(e, ue.Indexer, symbolTable);
                            // e.LdElemA();
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
    }
}