using System;
using System.Linq;
using ZenPlatform.Compiler.Contracts;
using ZenPlatform.Compiler.Contracts.Symbols;
using ZenPlatform.Compiler.Roslyn;
using ZenPlatform.Language.Ast.Definitions;
using ZenPlatform.Language.Ast.Symbols;
using ZenPlatform.Shared.Tree;
using Call = ZenPlatform.Language.Ast.Definitions.Call;
using Expression = ZenPlatform.Language.Ast.Definitions.Expression;

namespace ZenPlatform.Compiler.Generation
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