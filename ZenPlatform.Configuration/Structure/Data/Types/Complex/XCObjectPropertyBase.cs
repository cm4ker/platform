using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Portable.Xaml.Schema;
using ZenPlatform.Configuration.Structure.Data.Types.Primitive;
using ZenPlatform.Shared.ParenChildCollection;

namespace ZenPlatform.Configuration.Structure.Data.Types.Complex
{
    /// <summary>
    /// Если ваш компонент поддерживает свойства, их необходимо реализовывать через этот компонент
    /// </summary>
    public abstract class XCObjectPropertyBase
    {
        private List<XCTypeBase> _serializedTypes;
        private readonly List<XCTypeBase> _types;

        protected XCObjectPropertyBase()
        {
            _types = new List<XCTypeBase>();
            Guid = Guid.NewGuid();
            _serializedTypes = new List<XCTypeBase>();
        }

        /// <summary>
        /// Уникальный идентификатор свойства
        /// </summary>
        [XmlAttribute]
        public Guid Guid { get; set; }

        /// <summary>
        /// Уникальный идентификатор объекта в разрезе базы данных
        /// </summary>
        [XmlIgnore]
        public uint Id { get; set; }

        /// <summary>
        /// Указывает на то, является ли поле системным.
        /// Системные поля нельзя удалить напрямую, нельзя редактировать.
        /// На них можно лишь воздействовать через какие-нибудь другие свойства
        /// </summary>
        [XmlAttribute]
        public bool IsSystemProperty { get; set; }

        /// <summary>
        /// Вид даты (только для числовых типов)
        /// </summary>
        [XmlAttribute]
        public XCDateCaseType DateCase { get; set; }

        /// <summary>
        /// Псевдоним в системе
        /// </summary>
        [XmlAttribute]
        public string Name { get; set; }

        /// <summary>
        /// Длина только для Двоичных\Числовых\Строковых данных
        /// </summary>
        [XmlAttribute]
        public int Length { get; set; }

        /// <summary>
        /// Точность, только для числовых типов
        /// </summary>
        [XmlAttribute]
        public int Precision { get; set; }

        /// <summary>
        /// Уникальность, только для ключевых полей
        /// </summary>
        [XmlAttribute]
        public bool Unique { get; set; }


        /// <summary>
        /// Типы, описывающие поле
        /// </summary>
        [System.ComponentModel.DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public List<XCTypeBase> Types => _types;

        /// <summary>
        /// Поля для выгрузки конфигурации здесь происходит подмена XCObjectType на XCUnknownType
        /// </summary>
        [XCDecoratedForSerialization]
        internal List<XCTypeBase> SerializedTypes => _serializedTypes;

        private IEnumerable<XCTypeBase> GetTypes()
        {
            foreach (var type in Types)
            {
                if (type is XCPremitiveType) yield return type;
                if (type is XCObjectTypeBase objType) yield return new XCUnknownType() {Guid = objType.Guid};
            }
        }

        public bool ShouldSerializeSerializedTypes()
        {
            _serializedTypes = GetTypes().ToList();
            return true;
        }

        public List<XCTypeBase> GetUnprocessedPropertyTypes() => _serializedTypes;

        /// <summary>
        /// Колонка привязанная к базе данных. При загрузке должна присваиваться движком
        /// </summary>
        public string DatabaseColumnName { get; set; }

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
    }


    /// <summary>
    /// Коллекция свойст, предлагает расширение для класса ChildItemCollection
    /// </summary>
    /// <typeparam name="TBaseType">Тип базового объекта</typeparam>
    /// <typeparam name="TProperty">Тип элементов коллекции свойств</typeparam>
    public class XCPropertyCollection<TBaseType, TProperty> : ChildItemCollection<TBaseType, TProperty>
        where TProperty : XCObjectPropertyBase, IChildItem<TBaseType> where TBaseType : class
    {
        public XCPropertyCollection(TBaseType parent) : base(parent)
        {
        }

        public XCPropertyCollection(TBaseType parent, IList<TProperty> collection) : base(parent, collection)
        {
        }


        public TProperty GetProperty(Guid guid)
        {
            return this.FirstOrDefault(x => x.Guid == guid);
        }
    }
}