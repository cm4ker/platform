using System;
using ZenPlatform.Compiler.Contracts;
using ZenPlatform.Compiler.Contracts.Symbols;
using ZenPlatform.Compiler.Helpers;
using ZenPlatform.Compiler.Roslyn;
using ZenPlatform.Language.Ast.Definitions;
using ZenPlatform.Language.Ast.Infrastructure;
using ZenPlatform.Language.Ast.Symbols;
using Expression = ZenPlatform.Language.Ast.Definitions.Expression;

namespace ZenPlatform.Compiler.Generation
{
    public partial class Generator
    {
        private void LoadValue(IEmitter e, Block context)
        {
        }

        private void EmitVariable(RBlockBuilder e, SymbolTable symTable, Variable variable)
        {
            //load phase 

            if (variable.Value is Expression expr)
            {
                EmitExpression(e, expr, symTable);

                // if (_map.GetClrType(variable.Type).Equals(_bindings.Object))
                // {
                //     e.Box(_map.GetClrType(expr.Type));
                // }
            }
            else if (false) // variable.Value is ElementCollection ec)
            {
                // // Empty array initialization.
                // if (variable.Value != null && variable.Value is Expression value)
                // {
                //     EmitExpression(e, value, symTable);
                //     e.NewArr(_map.GetClrType(variable.Type).ArrayElementType);
                // }
                // else if (variable.Value != null)
                // {
                //     ElementCollection elements = null; //variable.Value as ElementCollection;
                //
                //     e.LdcI4(elements.Count);
                //     e.NewArr(_map.GetClrType(variable.Type).ArrayElementType);
                //
                //     for (int x = 0; x < elements.Count; x++)
                //     {
                //         // Load array
                //         e.Dup();
                //         // Load index
                //         e.LdcI4(x);
                //         // Load value
                //         EmitExpression(e, elements[x].Expression, symTable);
                //         // Store
                //         e.StElemI4();
                //     }
                // }
            }

            //store phase
            RLocal local = e.DefineLocal(_map.GetClrType(variable.Type));

            if (variable.Value is Expression ex)
            {
                if (variable.Value != null)
                {
                    e.StLoc(local);
                }
            }
            else if (false) //variable.Value is ElementCollection ec)
            {
                e.StLoc(local);
            }

            var symbol = symTable.Find<VariableSymbol>(variable);
            symbol.Connect(local);
        }
    }
}