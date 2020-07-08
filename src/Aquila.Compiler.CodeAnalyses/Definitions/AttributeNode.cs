using Aquila.Language.Ast.Infrastructure;
using Aquila.Language.Ast.Misc;

namespace Aquila.Language.Ast.Definitions
{
    /// <summary>
    /// Описывает  аттрибут
    /// </summary>
    public partial class AttributeSyntax : SyntaxNode
    {
        public AttributeSyntax(ILineInfo lineInfo, ArgumentList collection, Ast.SingleTypeSyntax type) : base(lineInfo)
        {
            Type = type;
        }

        public Ast.SingleTypeSyntax Type { get; set; }

        public ArgumentCollection Arguments { get; }
    }
}