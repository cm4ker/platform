namespace ZenPlatform.Compiler.AST.Definitions
{
    /// <summary>
    /// Представляет символ литера
    /// </summary>
    public class Literal : Infrastructure.Expression
    {
        /// <summary>
        /// Создать литерал
        /// </summary>
        /// <param name="value">Текстовое представление литерала</param>
        /// <param name="type">Тип литерала</param>
        public Literal(string value, ZType type)
        {
            Value = value;
            Type = type;
        }

        /// <summary>
        /// Текстовое представление литерала, строки и символы включают кавычки
        /// </summary>
        public string Value;
    }
}