using System;
using ZenPlatform.Compiler.AST.Definitions;
using ZenPlatform.Compiler.Contracts;
using ZenPlatform.Compiler.Sre;
using ZenPlatform.Language.Ast.AST.Definitions;

namespace ZenPlatform.Compiler.Visitor
{
    public class BasicVisitor : AstVisitorBase
    {
        private ITypeSystem _ts;

        public BasicVisitor(ITypeSystem ts)
        {
            _ts = ts;
        }

        public override void VisitSingleType(SingleTypeNode obj)
        {
            Console.Write($"We found type:{obj.Type.Name}, at {obj.Line}:{obj.Position} type: {obj.GetType()}");
            Console.WriteLine();

            if (obj.Type is UnknownArrayType)
            {
                obj.SetType(obj.Type.ArrayElementType.MakeArrayType());
            }
        }
    }
}