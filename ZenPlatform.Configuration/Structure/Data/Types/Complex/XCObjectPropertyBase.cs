using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Portable.Xaml.Schema;
using ZenPlatform.Configuration.Contracts;
using ZenPlatform.Configuration.Structure.Data.Types.Primitive;
using ZenPlatform.Language.Ast.Definitions.Expressions;

namespace ZenPlatform.Configuration.Structure.Data.Types.Complex
{
    public abstract class XCObjectPropertyBaseBase
    {
    }

    /// <summary>
    /// Если ваш компонент поддерживает свойства, их необходимо реализовывать через этот компонент
    /// </summary>
    [DebuggerDisplay("{" + nameof(Name) + "}")]
    public abstract class XCObjectPropertyBase : IXCProperty
    {
        private List<IXCType> _serializedTypes;
        private readonly List<IXCType> _types;

        protected XCObjectPropertyBase()
        {
            _types = new List<IXCType>();
            Guid = Guid.NewGuid();
            _serializedTypes = new List<IXCType>();
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
        /// Вид даты (только для числовых типов)
        /// </summary>
        //public XCDateCaseType DateCase { get; set; }

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
        [XmlIgnore]
        public string DatabaseColumnName { get; set; }

        /// <summary>
        /// Типы, описывающие поле
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public List<IXCType> Types => _types;

        public XCPropertyAccessPolicy AccessPolicy { get; set; }

        /// <summary>
        /// Поля для выгрузки конфигурации здесь происходит подмена XCObjectType на XCUnknownType
        /// </summary>
        [XCDecoratedForSerialization]
        internal List<IXCType> SerializedTypes => _serializedTypes;

        private IEnumerable<IXCType> GetTypes()
        {
            foreach (var type in Types)
            {
                if (type is IXCPrimitiveType) yield return type;
                if (type is XCObjectTypeBase objType) yield return new XCUnknownType() {Guid = objType.Guid};
                if (type is XCLinkTypeBase objLink) yield return new XCUnknownType() {Guid = objLink.Guid};
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
        public IEnumerable<IXCType> GetUnprocessedPropertyTypes() => _serializedTypes;

        /*
         * Ниже представлен алгоритм, как будет колонка разворачиваться в базу данных:
         *
         * DatabaseColumnName = Fld_035
         * |Fld_035_TypeId|Fld_035_TypeRef|Fld_035_Binary|Fld_035_Guid|Fld_035_Int|Fld_035_DateTime|Fld_035_String
         *
         * Когда Types.Count() == 1 и Types[0] is XCPrimitiveType
         * В таком случае выделяется единственная колонка колонка для хранения
         *      Guid || binary || bool || int || datetime
         * Когда Types.Count() == 1 и Types[0] is XCObjectType и Type.IsAabstract в таком случае выделяются две колонки
         *      IntTypeId, GuidRef
         * Когда Types.Count() == 1 и Types[0] is XCObjectType и !Type.IsAabstract в таком случае выделяется две или одна колонка.
         * Примечание. Всё зависит от того, есть ли унаследованные объекты от текущего объекта
         *      GuidRef \ IntTypeId, GuidRef
         * Когда Types.Count() > 1 и все Types is XCPrimitiveType
         *      В таком случае на каждый тип отводится своя колонка. Биндинг должен осуществляться таким
         *      не хитрым мапированием: Свойство, Тип -> Колонка
         */

        public virtual IEnumerable<XCColumnSchemaDefinition> GetPropertySchemas(string propName = null)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Свойство ссылки
    /// </summary>
    [DebuggerDisplay("{" + nameof(Name) + "}")]
    public class XCLinkProperty : IXCProperty
    {
        private readonly IXCLinkType _typeLink;
        private IXCProperty _idProp;
        private readonly uint _typeId;
        private List<IXCType> _types;

        public XCLinkProperty(IXCLinkType typeLink, IXCProperty idProp)
        {
            _typeLink = typeLink;
            _idProp = idProp;
            _typeId = typeLink.Id;

            _types = new List<IXCType> {_typeLink};
        }

        public Guid Guid { get; set; }
        public uint Id { get; set; }

        public bool IsSystemProperty
        {
            get => true;
            set => throw new NotImplementedException();
        }

        public bool IsSelfLink => true;

        public bool IsReadOnly
        {
            get => true;
            set => throw new NotImplementedException();
        }

        public string Name
        {
            get => "Link";
            set => throw new NotImplementedException();
        }

        public bool Unique
        {
            get => true;
            set => throw new NotImplementedException();
        }

        public List<IXCType> Types => _types;

        public XCPropertyAccessPolicy AccessPolicy
        {
            get => XCPropertyAccessPolicy.CanGetCode | XCPropertyAccessPolicy.CanGetDb;
            set => throw new NotImplementedException();
        }

        public string DatabaseColumnName
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        public IEnumerable<IXCType> GetUnprocessedPropertyTypes()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<XCColumnSchemaDefinition> GetPropertySchemas(string propName = null)
        {
            yield return new XCColumnSchemaDefinition(XCColumnSchemaType.Ref, new XCGuid(), _idProp.DatabaseColumnName);
            yield return new XCColumnSchemaDefinition(XCColumnSchemaType.Ref, new XCGuid(), _idProp.DatabaseColumnName);
        }
    }
}