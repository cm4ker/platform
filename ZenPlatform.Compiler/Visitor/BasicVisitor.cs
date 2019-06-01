using System;
using ZenPlatform.Compiler.AST.Definitions;
using ZenPlatform.Compiler.Contracts;
using ZenPlatform.Compiler.Sre;

namespace ZenPlatform.Compiler.Visitor
{
    public class BasicVisitor : AstVisitorBase
    {
        private ITypeSystem _ts;

        public BasicVisitor()
        {
            _ts = new SreTypeSystem();
        }

        public override void VisitType(TypeNode obj)
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