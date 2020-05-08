using Aquila.Language.Ast.Definitions;

namespace Aquila.Language.Ast
{
    public interface ITypedNode
    {
        /// <summary>
        /// Тип, вычисленный на основании AST дерева
        /// </summary>
        TypeSyntax Type { get; set; }
    }
}