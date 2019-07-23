using System;
using ZenPlatform.Compiler.Contracts.Symbols;
using ZenPlatform.Language.Ast.AST.Infrastructure;

namespace ZenPlatform.Language.Ast.AST.Definitions
{
    /// <summary>
    /// Описывает  аттрибут
    /// </summary>
    public class AttributeNode : SyntaxNode
    {
        public AttributeNode(ILineInfo lineInfo, ArgumentCollection collection, SingleTypeNode type) : base(lineInfo)
        {
            Type = type;
        }

        public SingleTypeNode Type { get; set; }

        public ArgumentCollection Arguments { get; }


        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitAttribute(this);
        }
    }
}