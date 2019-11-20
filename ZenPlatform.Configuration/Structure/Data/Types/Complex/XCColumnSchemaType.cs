namespace ZenPlatform.Configuration.Structure.Data.Types.Complex
{
    /// <summary>
    /// Детерминированный тип колонки реквизита конфигурации
    /// Реквизит может быть нескольких типов одновременно
    /// Это перечисление представляет все типы колонок которые могут быть 
    /// </summary>
    public enum XCColumnSchemaType
    {
        /// <summary>
        /// Не специализированная колонка. Говорит о том, что значение одно
        /// </summary>
        NoSpecial,

        /// <summary>
        /// Колонка значения (строка, число, дата и т.д.)
        /// </summary>
        Value,

        /// <summary>
        /// Колонка ссылки
        /// </summary>
        Ref,

        /// <summary>
        /// Колонка хранящая тип
        /// </summary>
        Type
    }
}