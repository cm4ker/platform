using ZenPlatform.Compiler.Contracts.Symbols;
using ZenPlatform.Language.Ast.AST.Infrastructure;

namespace ZenPlatform.Language.Ast.AST.Definitions
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


        public override void Accept<T>(IVisitor<T> visitor)
        {
            visitor.Visit(Type);
            foreach (var argument in Arguments)
                visitor.Visit(argument);
        }
    }
}