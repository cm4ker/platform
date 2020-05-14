using Aquila.Compiler.Contracts.Symbols;

namespace Aquila.Language.Ast.Definitions
{
    /// <summary>
    /// Представляет символ литера
    /// </summary>
    public partial class Literal
    {
        private TypeSyntax _type;

        /// <summary>
        /// Создать литерал
        /// </summary>
        /// <param name="value">Текстовое представление литерала</param>
        /// <param name="type">Тип литерала</param>
        /// <param name="isSqlLiteral"></param>
        public Literal(ILineInfo li, string value, TypeSyntax type, bool isSqlLiteral) : this(li, value, isSqlLiteral)
        {
            _type = type;
        }

        public override TypeSyntax Type
        {
            get => _type;
            set => _type = value;
        }

        /// <summary>
        /// Действительное значение
        /// </summary>
        public object ObjectiveValue { get; set; }
    }
}