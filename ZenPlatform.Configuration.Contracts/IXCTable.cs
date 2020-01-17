using System;
using System.Collections.Generic;

namespace ZenPlatform.Configuration.Contracts
{
    public interface IXCTable
    {
        /// <summary>
        /// Уникальный идентификатор свойства
        /// </summary>
        public Guid Guid { get; set; }

        /// <summary>
        /// Уникальный идентификатор объекта в разрезе базы данных
        /// </summary>
        public uint Id { get; set; }

        /// <summary>
        /// Владелец таблицы
        /// </summary>
        IXCObjectType ParentType { get; }

        /// <summary>
        /// Название таблицы
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Получить свойства табличной части. Если объект не поддерживает свойства будет выдано NotSupportedException
        /// </summary>
        /// <returns></returns>
        IEnumerable<IXCProperty> GetProperties();
    }
}