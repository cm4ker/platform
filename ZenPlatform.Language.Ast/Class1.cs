using System;
using System.Collections.Generic;
using System.Linq;
using ZenPlatform.Language.Ast.AST;
using ZenPlatform.Language.Ast.Definitions;
using ZenPlatform.Language.Ast.Definitions.Statements;

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

            var childs = node.Childs.ToArray();

            for (int i = 0; i < childs.Length; i++)
                (childs[i] as SyntaxNode).PrintPretty(indent, i == childs.Length - 1);
        }

        public static Block ToBlock(this Statement statement)
        {
            return new Block(new List<Statement>() {statement});
        }

        public static Statement ToStatement(this Expression exp)
        {
            return new ExpressionStatement(exp);
        }
    }
}