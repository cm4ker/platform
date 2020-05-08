using Aquila.Compiler.Contracts.Symbols;
using Aquila.Language.Ast.AST;
using Aquila.Language.Ast.Infrastructure;

namespace Aquila.Language.Ast.Definitions
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