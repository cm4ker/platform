using System;
using System.Linq;
using Aquila.Compiler.Contracts;
using Aquila.Compiler.Roslyn;
using Aquila.Language.Ast.Definitions;
using Aquila.Language.Ast.Symbols;
using Aquila.Shared.Tree;
using Call = Aquila.Language.Ast.Call;
using Expression = Aquila.Language.Ast.Expression;
using LookupExpression = Aquila.Language.Ast.LookupExpression;
using Name = Aquila.Language.Ast.Name;

namespace Aquila.Compiler.Generation
{
    public partial class Generator
    {
        private Node EmitGlobalVar(RBlockBuilder e, Node currentGv, Expression expr, SymbolTable symbolTable)
        {
            if (expr is Name n)
            {
                var gv = currentGv.Children.First(x =>
                {
                    var gvar = x as GlobalVarTreeItem;

                    if (gvar?.Type == VarTreeLeafType.Prop)
                        return gvar?.Name == n.Value;

                    return false;
                }) as GlobalVarTreeItem;

                if (n.Type is null || n.Type.Kind == TypeNodeKind.Unknown)
                {
                    n.Type = gv.AstType;
                    n.Attach(n.Type);
                }
                
                gv?.Emit(n, e);

                return gv;
            }
            else if (expr is Call c)
            {
                var gv = currentGv.Children.First(x =>
                {
                    var gvar = x as GlobalVarTreeItem;

                    if (gvar?.Type == VarTreeLeafType.Func)
                        return gvar?.Name == c.Name.Value;

                    return false;
                }) as GlobalVarTreeItem;

                EmitArguments(e, c.Arguments, symbolTable);
                if (c.Type is null || c.Type.Kind == TypeNodeKind.Unknown)
                {
                    c.Type = gv.AstType;
                    c.Attach(c.Type);
                }

                gv?.Emit(c, e);

                return gv;
            }
            else if (expr is LookupExpression le)
            {
                var last = EmitGlobalVar(e, currentGv, le.Current, symbolTable);
                return EmitGlobalVar(e, last, le.Lookup, symbolTable);
            }

            throw new Exception("Unknown expression in global space");
        }
    }
}