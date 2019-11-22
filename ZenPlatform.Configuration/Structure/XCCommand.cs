using System;
using System.Collections.Generic;
using System.Text;
using ZenPlatform.Configuration.Contracts;
using ZenPlatform.Configuration.Structure.Data.Types.Complex;

namespace ZenPlatform.Configuration.Structure
{
 
    /// <summary>
    /// Комманда
    /// </summary>
    public class XCCommand : IXCCommand
    {
        public XCCommand(bool predefined)
        {
            IsPredefined = predefined;
            Module = new XCProgramModule();
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

        /*
         * using Default()
         *
         * [ServerCommand]
         * Module.MyInterestedCommand()
         */

        public IXCProgramModule Module { get; set; }

        /// <summary>
        /// Аргументы команды
        /// </summary>
        public List<IXCDataExpression> Arguments { get; }
    }
}