using System;
using System.Collections.Generic;
using System.Text;
using ZenPlatform.Configuration.Contracts;

namespace ZenPlatform.EntityComponent.Configuration
{
    /// <summary>
    /// Редактор свойств 
    /// </summary>
    public class SingleEntityPropertyEditor
    {
        private XCSingleEntityProperty _property;

        public SingleEntityPropertyEditor(XCSingleEntityProperty property)
        {
            _property = property;
        }

        /// <summary>
        /// Установить имя
        /// </summary>
        /// <param name="name">Имя свойства</param>
        /// <returns></returns>
        public SingleEntityPropertyEditor SetName(string name)
        {
            _property.Name = name;
            return this;
        }

        /// <summary>
        /// Добавить тип
        /// </summary>
        /// <param name="type">Тип свойсва</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public SingleEntityPropertyEditor AddType(IXCType type)
        {
            if (type is IXCObjectReadOnlyType t && !t.IsLink)
                throw new Exception("Only link type can be used in another types");

            _property.Types.Add(type);

            return this;
        }

        /// <summary>
        /// Установить идентификатор
        /// </summary>
        /// <param name="guid">Идентификатор свойства</param>
        /// <returns></returns>
        public SingleEntityPropertyEditor SetGuid(Guid guid)
        {
            _property.Guid = guid;
            return this;
        }

        /// <summary>
        /// Установить имя колонки базы данных для данного свойства
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public SingleEntityPropertyEditor SetDatabaseColumnName(string name)
        {
            _property.DatabaseColumnName = name;
            return this;
        }
    }
}