using System;
using System.Collections.Generic;

namespace ZenPlatform.Configuration.Contracts
{
    public interface IXCCommand
    {
        /// <summary>
        /// Уникальный идентификатор комманды
        /// </summary>
        Guid Guid { get; set; }

        /// <summary>
        /// Предпределенная ли это команда (не доступна для редактирования)
        /// </summary>
        bool IsPredefined { get; }

        /// <summary>
        /// Текстовое представление комманды
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Явное отображение комманды в интерфейсе
        /// </summary>
        string DisplayName { get; set; }

        IMDProgramModule Module { get; set; }

        /// <summary>
        /// Аргументы команды
        /// </summary>
        List<IXCDataExpression> Arguments { get; }
    }

    public interface IComponentRef : IEquatable<IComponentRef>
    {
        string Name { get; set; }
        string Entry { get; set; }
    }
}