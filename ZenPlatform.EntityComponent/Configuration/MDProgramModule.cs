using ZenPlatform.Configuration.Contracts;

namespace ZenPlatform.EntityComponent.Configuration
{
    /// <summary>
    /// Программный модуль. Если необходимо организовать поддержку программных модулей на языке платформы
    /// В таком случае необходимо наследоваться от этого класса
    /// </summary>
    public class MDProgramModule
    {
        /// <summary>
        /// Имя модуля
        /// </summary>
        public string ModuleName { get; set; }

        /// <summary>
        /// Текст модуля
        /// </summary>
        public string ModuleText { get; set; }

        /// <summary>
        /// Тип программного модуля
        /// </summary>
        public ProgramModuleDirectionType ModuleDirectionType { get; set; }

        public ProgramModuleRelationType ModuleRelationType { get; set; }
    }
}