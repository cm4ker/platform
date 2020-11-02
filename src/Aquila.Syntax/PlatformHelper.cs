using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using Aquila.Syntax;
using Aquila.Syntax.Ast;
using Aquila.Syntax.Text;

public static class Helper
{
    public static Span ToLineInfo(this ParserRuleContext context)
    {
        return new Span(context.start.StartIndex, context.stop?.StopIndex+1 ?? 0 - context.start.StartIndex);
    }

    public static Span ToLineInfo(this IToken context)
    {
        return new Span(context.StartIndex, context.StopIndex - context.StartIndex);
    }

    public static IdentifierToken Identifier(this ITerminalNode node)
    {
        return new IdentifierToken(node.Symbol.ToLineInfo(), SyntaxKind.IdentifierToken, node.GetText());
    }


    // public static IEnumerable<Method> FilterFunc(this IEnumerable<Method> list, CompilationMode mode)
    // {
    //     return list.Where(x => ((int) x.Flags & (int) mode) != 0);
    // }
}
// }