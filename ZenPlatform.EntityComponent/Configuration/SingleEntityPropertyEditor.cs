using System;
using System.Collections.Generic;
using System.Text;
using ZenPlatform.Configuration.Contracts;

namespace ZenPlatform.EntityComponent.Configuration
{
    /// <summary>
    /// Редактор свойств 
    /// </summary>
    public class SingleEntityPropertyEditor : IPropertyEditor
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
        public IPropertyEditor SetName(string name)
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
        public IPropertyEditor AddType(IXCType type)
        {
            if (type is IXCStructureType t && !t.IsLink)
                throw new Exception("Only link type can be used in another types");

            _property.Types.Add(type);

            return this;
        }

        /// <summary>
        /// Установить идентификатор
        /// </summary>
        /// <param name="guid">Идентификатор свойства</param>
        /// <returns></returns>
        public IPropertyEditor SetGuid(Guid guid)
        {
            _property.Guid = guid;
            return this;
        }

        /// <summary>
        /// Установить имя колонки базы данных для данного свойства
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public IPropertyEditor SetDatabaseColumnName(string name)
        {
            _property.DatabaseColumnName = name;
            return this;
        }
    }


    public class SingleEntityTableEditor : ITableEditor
    {
        private readonly MDSingleEntityTable _table;

        public SingleEntityTableEditor(MDSingleEntityTable table)
        {
            _table = table;
        }

        public ITableEditor SetName(string name)
        {
            _table.Name = name;
            return this;
        }

        public IPropertyEditor CreateProperty()
        {
            var newProperty = new XCSingleEntityProperty();
            newProperty.Guid = Guid.NewGuid();
            _table.Properties.Add(newProperty);

            return new SingleEntityPropertyEditor(newProperty);
        }
    }
}