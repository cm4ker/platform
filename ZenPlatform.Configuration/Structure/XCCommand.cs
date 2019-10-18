using System.Collections.Generic;

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
        }

        // Предпределенная ли это команда (не доступна для редактирования)
        public bool IsPredefined { get; }

        // Текстовое представление комманды
        public string Name { get; set; }

        // Явное отображение комманды в интерфейсе
        public string Display { get; set; }

        // Какую процедуру выполняет комманда
        public string Handler { get; set; }

        // Аргументы команды
        public List<XCDataExpression> Arguments { get; }
    }
}