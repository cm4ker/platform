namespace ZenPlatform.Configuration.Contracts
{
    /// <summary>
    /// Менеджер конфигурации компонента. Отвечает за создание и удаление сущности
    /// </summary>
    public interface IXComponentManager
    {
        /// <summary>
        /// Создать новую сущност
        /// </summary>
        /// <param name="parent">Родитель этой сущности</param>
        /// <returns>Новый объект конфигурации</returns>
        IXCObjectType Create(IXCObjectType baseType = null);

        /// <summary>
        /// Удалить объект конфигурации. Причём конфигурация остаётся в целостном состоянии до и после удаления
        /// </summary>
        /// <param name="type"></param>
        void Delete(IXCObjectType type);
    }
}