using System.Collections.Generic;
using ZenPlatform.Language.Ast;
using ZenPlatform.Language.Ast.AST;
using ZenPlatform.Language.Ast.Definitions;

namespace ZenPlatform.Compiler.Tests
{
    public class AstWalkerTest
    {
        public class SimpleAstWalker : AstVisitorBase<SyntaxNode>
        {
            public SimpleAstWalker()
            {
                FoundedLiterals = new List<string>();
            }

            public List<string> FoundedLiterals { get; }

            public override SyntaxNode VisitLiteral(Literal obj)
            {
                FoundedLiterals.Add(obj.Value);
                return base.VisitLiteral(obj);
            }
        }


        public void SimpleLiteralTest()
        {
            var test = 
@"module Test 
{
    void SomeVoidName()
    {
        string var1 = ""First literal"";
    }
}";
        }
    }
}