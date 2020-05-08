namespace Aquila.Pidl.ObjectModel
{
    /// <summary>
    /// Поле
    /// </summary>
    public class Field : Element
    {
        /// <summary>
        /// Расширение поля
        /// </summary>
        public object Extension { get; set; }

        /// <summary>
        /// Выражение источника данных
        /// </summary>
        public string Source { get; set; }
    }
}
