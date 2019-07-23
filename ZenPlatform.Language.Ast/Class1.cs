using System;
using System.Linq;
using ZenPlatform.Language.Ast.AST;

namespace ZenPlatform.Language.Ast
{
    public static class AstNodeExtensions
    {
        public static void PrintPretty(this SyntaxNode node, string indent, bool last)
        {
            Console.Write(indent);
            if (last)
            {
                Console.Write("\\-");
                indent += "  ";
            }
            else
            {
                Console.Write("|-");
                indent += "| ";
            }

            Console.WriteLine(node.GetType().Name);

            var childs = node.Children.ToArray();

            for (int i = 0; i < childs.Length; i++)
                childs[i].PrintPretty(indent, i == childs.Length - 1);
        }
    }
}