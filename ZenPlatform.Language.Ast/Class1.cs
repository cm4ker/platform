using System;
using System.Collections;
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
            return new Block(new StatementList {statement});
        }

        public static T ToAstList<T, TC>(this IEnumerable<TC> items)
            where TC : SyntaxNode
            where T : SyntaxCollectionNode<TC>, new()
        {
            var result = new T();

            foreach (var item in items)
            {
                result.Add(item);
            }

            return result;
        }

        public static Statement ToStatement(this Expression exp)
        {
            return new ExpressionStatement(exp);
        }
    }
}