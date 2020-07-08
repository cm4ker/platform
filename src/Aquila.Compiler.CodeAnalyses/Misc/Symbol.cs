namespace Aquila.Language.Ast.Misc
{
    /// <summary>
    /// Тип пространства, где доступен данный символ
    /// </summary>
    public enum SymbolScopeBySecurity
    {
        /// <summary>
        /// Оыбласть видимости не известна
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// Пользовательское
        /// </summary>
        User = 1,

        /// <summary>
        /// Системное
        /// </summary>
        System = 2 >> 1,

        /// <summary>
        /// Символ виден и для системного и для пользовательского использования
        /// </summary>
        Shared = User | System,
    }
}