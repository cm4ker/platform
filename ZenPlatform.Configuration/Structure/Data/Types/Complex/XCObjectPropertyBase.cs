using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Portable.Xaml.Schema;
using ZenPlatform.Configuration.Structure.Data.Types.Primitive;

namespace ZenPlatform.Configuration.Structure.Data.Types.Complex
{
    /// <summary>
    /// Если ваш компонент поддерживает свойства, их необходимо реализовывать через этот компонент
    /// </summary>
    [DebuggerDisplay("{" + nameof(Name) + "}")]
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
        public bool IsSystemProperty { get; set; }

        /// <summary>
        /// Указывает на то, что поле является только для
        /// чтения
        /// </summary>
        public bool IsReadOnly { get; set; }

        /// <summary>
        /// Вид даты (только для числовых типов)
        /// </summary>
        public XCDateCaseType DateCase { get; set; }

        /// <summary>
        /// Псевдоним в системе
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Длина только для Двоичных\Числовых\Строковых данных
        /// </summary>
        public int Length { get; set; }

        /// <summary>
        /// Точность, только для числовых типов
        /// </summary>
        public int Precision { get; set; }

        /// <summary>
        /// Уникальность, только для ключевых полей
        /// </summary>
        public bool Unique { get; set; }


        /// <summary>
        /// Типы, описывающие поле
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
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
                if (type is XCPrimitiveType) yield return type;
                if (type is XCObjectTypeBase objType) yield return new XCUnknownType() {Guid = objType.Guid};
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

        /// <summary>
        /// Получить необработанные типы свойств. Вызывается во время конструирования типа при загрузке конфигурации.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<XCTypeBase> GetUnprocessedPropertyTypes() => _serializedTypes;

        /// <summary>
        /// Колонка привязанная к базе данных. При загрузке должна присваиваться движком
        /// </summary>
        [XmlIgnore]
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

        public IEnumerable<XCColumnSchemaDefinition> GetPropertySchemas(string propName = null)
        {
            if (string.IsNullOrEmpty(propName)) propName = this.DatabaseColumnName;

            var done = false;

            if (Types.Count == 1)
                yield return new XCColumnSchemaDefinition(XCColumnSchemaType.NoSpecial, Types[0], propName, false);
            if (Types.Count > 1)
            {
                yield return new XCColumnSchemaDefinition(XCColumnSchemaType.Type, null, propName,
                    false, "", "_Type");

                foreach (var type in _types)
                {
                    if (type is XCPrimitiveType)
                        yield return new XCColumnSchemaDefinition(XCColumnSchemaType.Value, type,
                            propName, false, "", $"_{type.Name}");

                    if (type is XCObjectTypeBase obj && !done)
                    {
                        yield return new XCColumnSchemaDefinition(XCColumnSchemaType.Ref, type, propName,
                            !obj.Parent.ComponentImpl.DatabaseObjectsGenerator.HasForeignColumn, "", "_Ref");

                        done = true;
                    }
                }
            }
        }
    }


    /// <summary>
    /// Описывает тип и название колонки
    /// </summary>
    public struct XCColumnSchemaDefinition
    {
        public XCColumnSchemaDefinition(XCColumnSchemaType schemaType, XCTypeBase platformType, string name,
            bool isPseudo, string prefix = "", string postfix = "")
        {
            SchemaType = schemaType;
            Name = name;
            PlatformType = platformType;
            IsPseudo = isPseudo;
            Prefix = prefix ?? throw new ArgumentNullException();
            Postfix = postfix ?? throw new ArgumentNullException();
        }

        /// <summary>
        /// Тип колонки
        /// </summary>
        public XCColumnSchemaType SchemaType { get; set; }

        /// <summary>
        /// Полное название
        /// </summary>
        public string FullName => $"{Prefix}{Name}{Postfix}";

        public string Name { get; }

        /// <summary>
        /// Префикс
        /// </summary>
        public string Prefix { get; }

        /// <summary>
        /// Постфикс
        /// </summary>
        public string Postfix { get; }

        /// <summary>
        /// Тип платформы, закреплённый за схемой
        /// </summary>
        public XCTypeBase PlatformType { get; set; }

        /// <summary>
        /// Псевдо схема. Используется, если "чужие" свойства не создают колонки, но программного должны генерироваться
        /// </summary>
        public bool IsPseudo { get; set; }
    }

    /// <summary>
    /// Детерминированный тип колонки реквизита конфигурации
    /// Реквизит может быть нескольких типов одновременно
    /// Это перечисление представляет все типы колонок которые могут быть 
    /// </summary>
    public enum XCColumnSchemaType
    {
        /// <summary>
        /// Не специализированная колонка. Говорит о том, что значение одно
        /// </summary>
        NoSpecial,

        /// <summary>
        /// Колонка значения (строка, число, дата и т.д.)
        /// </summary>
        Value,

        /// <summary>
        /// Колонка ссылки
        /// </summary>
        Ref,

        /// <summary>
        /// Колонка хранящая тип
        /// </summary>
        Type
    }
}