using ZenPlatform.Compiler.Contracts.Symbols;
using ZenPlatform.Language.Ast.AST.Infrastructure;

namespace ZenPlatform.Language.Ast.AST.Definitions
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
        public Literal(ILineInfo li, string value, TypeNode type) : base(li)
        {
            Value = value;
            Type = type;
        }

        /// <summary>
        /// Текстовое представление литерала, строки и символы включают кавычки
        /// </summary>
        public string Value;

        public override void Accept<T>(IVisitor<T> visitor)
        {
            visitor.Visit(Type);
        }
    }
}