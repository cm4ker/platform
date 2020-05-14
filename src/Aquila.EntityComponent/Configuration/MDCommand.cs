using System;
using System.Collections.Generic;
using Aquila.Configuration.Contracts;
using Aquila.Configuration.Structure;

namespace Aquila.EntityComponent.Configuration
{
    /// <summary>
    /// Комманда
    /// </summary>
    public class MDCommand
    {
        public MDCommand()
        {
        }

        public MDCommand(bool predefined)
        {
            IsPredefined = predefined;
            Module = new MDProgramModule();
            Guid = Guid.NewGuid();
        }

        /// <summary>
        /// Уникальный идентификатор комманды
        /// </summary>
        public Guid Guid { get; set; }

        /// <summary>
        /// Предпределенная ли это команда (не доступна для редактирования)
        /// </summary>
        public bool IsPredefined { get; }

        /// <summary>
        /// Текстовое представление комманды
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Явное отображение комманды в интерфейсе
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Программный модуль комманды
        /// </summary>
        public MDProgramModule Module { get; set; }

        /// <summary>
        /// Аргументы команды
        /// </summary>
        public List<IXCDataExpression> Arguments { get; }
    }
}