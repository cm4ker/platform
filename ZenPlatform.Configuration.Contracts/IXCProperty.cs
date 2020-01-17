using System;
using System.Collections.Generic;

namespace ZenPlatform.Configuration.Contracts
{
    [Flags]
    public enum XCPropertyAccessPolicy
    {
        CanGetCode = 1 << 0,
        CanGetDb = 1 << 1,

        CanSetCode = 1 << 2,
        CanSetDb = 1 << 3
    }

    public interface IXCProperty
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
        /// Указывает на то, является ли поле системным.
        /// Системные поля нельзя удалить напрямую, нельзя редактировать.
        /// На них можно лишь воздействовать через какие-нибудь другие свойства
        /// </summary>
        bool IsSystemProperty { get; set; }

        /// <summary>
        /// Свойство является ссылкой
        /// </summary>
        bool IsSelfLink { get; }
        
        /// <summary>
        /// Указывает на то, что поле является только для
        /// чтения
        /// </summary>
        bool IsReadOnly { get; set; }

        /// <summary>
        /// Псевдоним в системе
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Уникальность, только для ключевых полей
        /// </summary>
        bool Unique { get; set; }

        /// <summary>
        /// Типы, описывающие поле
        /// </summary>
        List<IXCType> Types { get; }

        /// <summary>
        /// Настройки доступа к полю на всех этапах
        /// </summary>
        XCPropertyAccessPolicy AccessPolicy { get; set; }

        /// <summary>
        /// Колонка привязанная к базе данных. При загрузке должна присваиваться движком
        /// </summary>
        string DatabaseColumnName { get; set; }

        /// <summary>
        /// Получить необработанные типы свойств. Вызывается во время конструирования типа при загрузке конфигурации.
        /// </summary>
        /// <returns></returns>
        IEnumerable<IXCType> GetUnprocessedPropertyTypes();

        IEnumerable<XCColumnSchemaDefinition> GetPropertySchemas(string propName = null);
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

    /// <summary>
    /// Описывает тип и название колонки
    /// </summary>
    public class XCColumnSchemaDefinition
    {
        public XCColumnSchemaDefinition(XCColumnSchemaType schemaType, IXCType platformType, string name,
            string prefix = "", string postfix = "")
        {
            SchemaType = schemaType;
            Name = name;
            PlatformType = platformType;
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
        public IXCType PlatformType { get; set; }
    }
}