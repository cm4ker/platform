using ZenPlatform.Compiler.Contracts.Symbols;

namespace ZenPlatform.Language.Ast.Definitions
{
    /// <summary>
    /// Представляет символ литера
    /// </summary>
    public partial class Literal
    {
        /// <summary>
        /// Создать литерал
        /// </summary>
        /// <param name="value">Текстовое представление литерала</param>
        /// <param name="type">Тип литерала</param>
        public Literal(ILineInfo li, string value, TypeSyntax type) : this(li)
        {
            Value = value;
            Type = type;
        }


        /// <summary>
        /// Текстовое представление литерала, строки и символы включают кавычки
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Действительное значение
        /// </summary>
        public object ObjectiveValue { get; set; }
    }
}