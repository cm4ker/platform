using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using ZenPlatform.Configuration.ConfigurationLoader.Contracts;
using ZenPlatform.Configuration.ConfigurationLoader.XmlConfiguration.Data.Types;

namespace ZenPlatform.Configuration.ConfigurationLoader.XmlConfiguration
{
    /// <summary>
    /// Если ваш компонент поддерживает свойства, их необходимо реализовывать через этот компонент
    /// </summary>
    public abstract class XCObjectPropertyBase
    {
        /// <summary>
        /// Уникальный идентификатор свойства
        /// </summary>
        [XmlAttribute] public Guid Id { get; set; }

        /// <summary>
        /// Указывает на то, является ли поле системным.
        /// Системные поля нельзя удалить напрямую, нельзя редактировать.
        /// На них можно лишь воздействовать через какие-нибудь другие свойства
        /// </summary>
        [XmlAttribute] public bool IsSystemProperty { get; set; }

        /// <summary>
        /// Вид даты (только для числовых типов)
        /// </summary>
        [XmlAttribute] public XCDateCaseType DateCase { get; set; }

        /// <summary>
        /// Псевдоним в системе
        /// </summary>
        [XmlAttribute] public string Alias { get; set; }

        /// <summary>
        /// Длина только для Двоичных\Числовых\Строковых данных
        /// </summary>
        [XmlAttribute] public int Length { get; set; }

        /// <summary>
        /// Точность, только для числовых типов
        /// </summary>
        [XmlAttribute] public int Precision { get; set; }

        /// <summary>
        /// Уникальность, только для ключевых полей
        /// </summary>
        [XmlAttribute] public bool Unique { get; set; }

        [XmlArray]
        [XmlArrayItem(ElementName = "Type")]
        public List<XCTypeBase> Types { get; }

        /// <summary>
        /// Колонка привязанная к базе данных
        /// </summary>
        [XmlAttribute]
        public string DatabaseColumnName { get; set; }
        /*
         * DatabaseColumnName = Fld_035
         * |Fld_035_TypeId|Fld_035_TypeRef|Fld_035_Binary|Fld_035_Guid|Fld_035_Int|Fld_035_DateTime|Fld_035_String
         */
        /*
         * Когда Types.Count() == 1 и Types[0] is XCPrimitiveType
         * В таком случае выделяется единственная колонка колонка для хранения
         *      * Guid || binary || bool || int || datetime
         * Когда Types.Count() == 1 и Types[0] is XCObjectType и Type.IsAabstract В таком случае выделяются две колонки
         *      * IntTypeId, GuidRef
         * Когда Types.Count() == 1 и Types[0] is XCObjectType и не Type.IsAabstract В таком случае выделяются две колонки
         *      GuidRef
         * Когда Types.Count() > 1 и все Types is XCPrimitiveType
         *      В таком случае на каждый тип отводится своя колонка. Биндинг должен осуществляться таким
         *      не хитрым мапированием: Свойство, Тип -> Колонка
         */

    }
}