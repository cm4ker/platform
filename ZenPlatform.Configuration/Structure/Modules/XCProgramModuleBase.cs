namespace ZenPlatform.Configuration.Structure
{
    /// <summary>
    /// Программный модуль. Если необходимо организовать поддержку программных модулей на языке платформы
    /// В таком случае необходимо наследоваться от этого класса
    /// </summary>
    public abstract class XCProgramModuleBase
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
        public XCProgramModuleDirectionType ModuleDirectionType { get; set; }

        public XCProgramModuleRelationType ModuleRelationType { get; set; }
    }
}