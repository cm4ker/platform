using System;
using System.Collections.Generic;

namespace ZenPlatform.Configuration.Contracts
{
    public interface IXCTable
    {
        /// <summary>
        /// Уникальный идентификатор свойства
        /// </summary>
        Guid Guid { get; set; }

        /// <summary>
        /// Уникальный идентификатор объекта в разрезе базы данных
        /// </summary>
        uint Id { get; set; }

        /// <summary>
        /// Владелец таблицы
        /// </summary>
        IXCObjectType ParentType { get; }

        /// <summary>
        /// Наз
        /// </summary>
        string RelTableName { get; }

        /// <summary>
        /// Название таблицы
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Получить свойства табличной части. Если объект не поддерживает свойства будет выдано NotSupportedException
        /// </summary>
        /// <returns></returns>
        IEnumerable<IXCProperty> GetProperties();
    }
}