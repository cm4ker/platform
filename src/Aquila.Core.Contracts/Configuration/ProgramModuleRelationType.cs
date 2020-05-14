namespace Aquila.Core.Contracts.Configuration
{
    /// <summary>
    /// Тип модуля по отношению к объекту
    /// </summary>
    public enum ProgramModuleRelationType
    {
        /// <summary>
        /// Модуль относится непосредственно к объекту
        /// </summary>
        Object,

        /// <summary>
        /// Модуль относится к менеджеру объектов
        /// </summary>
        Manager
    }
}