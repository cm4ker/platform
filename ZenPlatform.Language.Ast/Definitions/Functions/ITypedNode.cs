namespace ZenPlatform.Language.Ast.Definitions.Functions
{
    public interface ITypedNode
    {
        /// <summary>
        /// Тип, вычисленный на основании AST дерева
        /// </summary>
        TypeSyntax Type { get; set; }
    }
}