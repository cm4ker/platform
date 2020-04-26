using ZenPlatform.Compiler.Contracts.Symbols;
using ZenPlatform.Language.Ast.AST;
using ZenPlatform.Language.Ast.Infrastructure;

namespace ZenPlatform.Language.Ast.Definitions
{
    /// <summary>
    /// Описывает  аттрибут
    /// </summary>
    public partial class AttributeSyntax : SyntaxNode
    {
        public AttributeSyntax(ILineInfo lineInfo, ArgumentList collection, SingleTypeSyntax type) : base(lineInfo)
        {
            Type = type;
        }

        public SingleTypeSyntax Type { get; set; }

        public ArgumentCollection Arguments { get; }
    }
}