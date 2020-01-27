using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Xml.Serialization;
using ZenPlatform.Configuration.Contracts;
using ZenPlatform.Configuration.Contracts.TypeSystem;
using ZenPlatform.Configuration.TypeSystem;

namespace ZenPlatform.Configuration.Structure.Data.Types.Complex
{
    /// <summary>
    /// Если ваш компонент поддерживает свойства, их необходимо реализовывать через этот компонент
    /// </summary>
    [DebuggerDisplay("{" + nameof(Name) + "}")]
    public class MDProperty : IMDProperty
    {
        private List<IMDType> _serializedTypes;
        private readonly List<IMDType> _types;

        protected MDProperty()
        {
            _types = new List<IMDType>();
            Guid = Guid.NewGuid();
            _serializedTypes = new List<IMDType>();
        }

        /// <summary>
        /// Уникальный идентификатор свойства
        /// </summary>
        public Guid Guid { get; set; }

        /// <summary>
        /// Уникальный идентификатор объекта в разрезе базы данных
        /// </summary>
        public uint Id { get; set; }

        /// <summary>
        /// Указывает на то, является ли поле системным.
        /// Системные поля нельзя удалить напрямую, нельзя редактировать.
        /// На них можно лишь воздействовать через какие-нибудь другие свойства
        /// </summary>
        public virtual bool IsSystemProperty { get; set; }

        public virtual bool IsSelfLink => false;

        /// <summary>
        /// Указывает на то, что поле является только для
        /// чтения
        /// </summary>
        public bool IsReadOnly { get; set; }

        /// <summary>
        /// Псевдоним в системе
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Уникальность, только для ключевых полей
        /// </summary>
        public bool Unique { get; set; }

        /// <summary>
        /// Колонка привязанная к базе данных. При загрузке должна присваиваться движком
        /// </summary>
        public string DatabaseColumnName { get; set; }

        /// <summary>
        /// Типы, описывающие поле
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public List<IMDType> Types => _types;

        /// <summary>
        /// Поля для выгрузки конфигурации здесь происходит подмена XCObjectType на XCUnknownType
        /// </summary>
        [XCDecoratedForSerialization]
        internal List<IMDType> SerializedTypes => _serializedTypes;

        private IEnumerable<IMDType> GetTypes()
        {
            return null;
            foreach (var type in Types)
            {
                // if (type is IXCPrimitiveType) yield return type;
                // else yield return new RefType {Guid = type.Guid};
            }
        }

        /// <summary>
        /// Проверка должно ли сериализоваться свойство типов
        /// </summary>
        /// <returns></returns>
        private bool ShouldSerializeSerializedTypes()
        {
            _serializedTypes = GetTypes().ToList();
            return true;
        }

        private bool ShouldSerializeId()
        {
            return false;
        }

        /// <summary>
        /// Получить необработанные типы свойств. Вызывается во время конструирования типа при загрузке конфигурации.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IMDType> GetUnprocessedPropertyTypes() => _serializedTypes;

        public virtual IEnumerable<XCColumnSchemaDefinition> GetPropertySchemas(string propName = null)
        {
            throw new NotImplementedException();
        }
    }
}