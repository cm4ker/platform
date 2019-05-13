namespace ZenPlatform.Compiler
{
    /// <summary>
    /// Расширение языка
    /// </summary>
    public interface IAstLanguageExtension
    {
        /// <summary>
        /// Метод трансформирует одно AST дерево в другое 
        /// </summary>
        /// <param name="item">Трансформируемое дерево</param>
        /// <returns></returns>
        IAstItem Transform(IAstItem item);
    }

    public interface IAstItem
    {
    }
}