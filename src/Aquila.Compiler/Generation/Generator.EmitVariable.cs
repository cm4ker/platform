using System;
using Aquila.Compiler.Contracts;
using Aquila.Compiler.Contracts.Symbols;
using Aquila.Compiler.Helpers;
using Aquila.Compiler.Roslyn;
using Aquila.Language.Ast.Definitions;
using Aquila.Language.Ast.Infrastructure;
using Aquila.Language.Ast.Symbols;
using Expression = Aquila.Language.Ast.Definitions.Expression;

namespace Aquila.Compiler.Generation
{
    public partial class Generator
    {
        private void LoadValue(IEmitter e, Block context)
        {
        }

        private void EmitVariable(RoslynEmitter e, SymbolTable symTable, Variable variable)
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

            if (variable.Value == null || variable.Type == null || variable.Type.Kind == TypeNodeKind.Unknown)
            {
                throw new Exception(
                    $"variable L:{variable.Line}P:{variable.Position} {variable.Name} type is unknown");
            }

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