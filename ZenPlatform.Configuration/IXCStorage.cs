namespace ZenPlatform.Configuration
{

    /// <summary>
    /// Стратегия загрузки/сохранения данных конфигурации.
    /// Есть возмо
    /// </summary>
    public interface IXCStorage
    {
        /// <summary>
        /// Получить двоичный объект
        /// </summary>
        /// <param name="name">Имя объекта</param>
        /// <param name="route">Параметр маршрутизации</param>
        /// <returns></returns>
        byte[] GetBlob(string name, string route);

        /// <summary>
        /// Получить строковый объект
        /// </summary>
        /// <param name="name">Имя объекта</param>
        /// <param name="route">Параметр маршрутизации</param>
        /// <returns></returns>
        string GetStringBlob(string name, string route);

        /// <summary>
        /// Сохранить двоичный объект
        /// </summary>
        /// <param name="name">Имя объекта</param>
        /// <param name="route">Параметр маршрутизации</param>
        /// <param name="bytes">Данные</param>
        void SaveBlob(string name, string route, byte[] bytes);

        /// <summary>
        /// Сохранить строковый объект
        /// </summary>
        /// <param name="name">Имя объекта</param>
        /// <param name="route">Параметр маршрутизации</param>
        /// <param name="data">Строковые данные</param>
        void SaveBlob(string name, string route, string data);

        byte[] GetRootBlob();

        string GetStringRootBlob();
    }
}