using System;
using Microsoft.CodeAnalysis;
using ZenPlatform.Compiler.Generation.NewGenerator;
using ZenPlatform.Compiler.Infrastructure;
using ZenPlatform.Language.Ast.AST.Definitions;
using ZenPlatform.Language.Ast.AST.Definitions.Expressions;
using ZenPlatform.Language.Ast.AST.Infrastructure;

namespace ZenPlatform.Compiler.Tests
{
    class Program
    {
        static void Main(string[] args)
        {
            VRoslyn r = new VRoslyn();

            var t = r.VisitLogicalOrArithmeticExpression(
                new LogicalOrArithmeticExpression(null, new Name(null, "Test"), UnaryOperatorType.Positive));

            Console.WriteLine(t.NormalizeWhitespace().ToFullString());
        }
    }
}