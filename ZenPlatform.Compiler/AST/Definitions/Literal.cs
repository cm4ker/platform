using ZenPlatform.Compiler.AST.Infrastructure;
using ZenPlatform.Compiler.Contracts;

namespace ZenPlatform.Compiler.AST.Definitions
{
    /// <summary>
    /// Представляет символ литера
    /// </summary>
    public class Literal : Expression
    {
        /// <summary>
        /// Создать литерал
        /// </summary>
        /// <param name="value">Текстовое представление литерала</param>
        /// <param name="type">Тип литерала</param>
        public Literal(ILineInfo li, string value, IType type) : base(li)
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