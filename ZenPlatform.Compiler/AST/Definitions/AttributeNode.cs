using ZenPlatform.Compiler.AST.Infrastructure;
using ZenPlatform.Compiler.Visitor;

namespace ZenPlatform.Compiler.AST.Definitions
{
    /// <summary>
    /// Описывает  аттрибут
    /// </summary>
    public class AttributeNode : AstNode
    {
        public AttributeNode(ILineInfo lineInfo, ArgumentCollection collection, TypeNode type) : base(lineInfo)
        {
            Type = type;
        }

        public TypeNode Type { get; set; }

        public ArgumentCollection Arguments { get; }


        public override void Accept(IVisitor visitor)
        {
            visitor.Visit(Type);
            Arguments.ForEach(visitor.Visit);
        }
    }
}