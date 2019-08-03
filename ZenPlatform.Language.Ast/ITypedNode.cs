using ZenPlatform.Language.Ast.Definitions;

namespace ZenPlatform.Language.Ast
{
    public interface ITypedNode
    {
        /// <summary>
        /// Тип, вычисленный на основании AST дерева
        /// </summary>
        TypeSyntax Type { get; set; }
    }
}