namespace ZenPlatform.Compiler.Contracts.Symbols
{
    public enum SymbolType
    {
        /// <summary>
        /// Неопознанный символ
        /// </summary>
        None,

        /// <summary>
        /// Функция
        /// </summary>
        Method,

        /// <summary>
        /// Свойсство
        /// </summary>
        Property,

        /// <summary>
        /// Тип
        /// </summary>
        Type,

        /// <summary>
        /// Переменная
        /// </summary>
        Variable,

        /// <summary>
        /// Конструктор
        /// </summary>
        Constructor
    }
}