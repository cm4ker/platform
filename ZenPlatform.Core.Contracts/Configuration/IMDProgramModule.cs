namespace ZenPlatform.Configuration.Contracts
{
    public interface IMDProgramModule
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
        ProgramModuleDirectionType ModuleDirectionType { get; set; }

        ProgramModuleRelationType ModuleRelationType { get; set; }
    }
}