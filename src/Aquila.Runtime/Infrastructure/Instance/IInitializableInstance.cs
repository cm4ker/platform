namespace Aquila.Core.Contracts.Instance
{
    /// <summary>
    ///  Среда для подключения. Обеспечивает некий контекст в котором работает удаленный пользователь
    /// </summary>
    public interface IInitializableInstance<in TConfiguration> : IInstance
        where TConfiguration : class
    {
        /// <summary>
        /// Инициализация
        /// </summary>
        /// <param name="config">Конфигурация</param>
        void Initialize(TConfiguration config);
    }
}