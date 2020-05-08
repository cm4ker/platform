using Aquila.Compiler.Contracts.Symbols;

namespace Aquila.Compiler
{
    /// <summary>
    /// Расширение языка
    /// </summary>
    public interface IAstLanguageExtension
    {
        /// <summary>
        /// Метод трансформирует одно AST дерево в другое 
        /// </summary>
        /// <param name="node">Трансформируемое дерево</param>
        /// <returns></returns>
        IAstNode Transform(IAstNode node);
    }
}