namespace ZenPlatform.Configuration.Contracts
{
    public interface IXCProgramModule
    {
        /// <summary>
        /// Имя модуля
        /// </summary>
        string ModuleName { get; set; }

        /// <summary>
        /// Текст модуля
        /// </summary>
        string ModuleText { get; set; }

        /// <summary>
        /// Тип программного модуля
        /// </summary>
        XCProgramModuleDirectionType ModuleDirectionType { get; set; }

        XCProgramModuleRelationType ModuleRelationType { get; set; }
    }
}