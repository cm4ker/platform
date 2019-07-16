namespace ZenPlatform.Language.Ast.AST.Definitions.Functions
{
    public interface ITypedNode
    {
        /// <summary>
        /// Тип, вычисленный на основании AST дерева
        /// </summary>
        TypeNode Type { get; set; }
    }
}