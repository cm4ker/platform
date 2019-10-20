using System;
using System.Collections.Generic;
using System.Text;

namespace ZenPlatform.Configuration.Structure
{
    /// <summary>
    /// Комманда
    /// </summary>
    public class XCCommand
    {
        public XCCommand(bool predefined)
        {
            IsPredefined = predefined;

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
        public string Display { get; set; }

        /// <summary>
        /// Какую процедуру выполняет комманда
        ///
        /// В формате (Module.FunctionName) 
        /// </summary>
        public string Handler { get; set; }

        /*
         * using Default()
         *
         * [ServerCommand]
         * Module.MyInterestedCommand()
         */
        
        /// <summary>
        /// Аргументы команды
        /// </summary>
        public List<XCDataExpression> Arguments { get; }
    }
}