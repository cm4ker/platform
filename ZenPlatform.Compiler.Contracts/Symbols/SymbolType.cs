using System;

namespace ZenPlatform.Compiler.Contracts.Symbols
{
    [Flags]
    public enum SymbolType
    {
        /// <summary>
        /// Неопознанный символ
        /// </summary>
        None = 0x0,

        /// <summary>
        /// Функция
        /// </summary>
        Method = 1 << 1,

        /// <summary>
        /// Свойсство
        /// </summary>
        Property = 1 << 2,

        /// <summary>
        /// Тип
        /// </summary>
        Type = 1 << 3,

        /// <summary>
        /// Переменная
        /// </summary>
        Variable = 1 << 4,

        /// <summary>
        /// Конструктор
        /// </summary>
        Constructor = 1 << 5
    }
}