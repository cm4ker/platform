using System;
using Aquila.Metadata;

namespace Aquila.Migrations
{
    /// <summary>
    /// Детерминированный тип колонки реквизита конфигурации
    /// Реквизит может быть нескольких типов одновременно
    /// Это перечисление представляет все типы колонок которые могут быть 
    /// </summary>
    public enum ColumnSchemaType
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
    public class ColumnSchemaDefinition
    {
        public ColumnSchemaDefinition(ColumnSchemaType schemaType, string name, SMType type,
            string prefix = "", string postfix = "")
        {
            SchemaType = schemaType;
            Name = name;
            Prefix = prefix ?? throw new ArgumentNullException();
            Postfix = postfix ?? throw new ArgumentNullException();
            Type = type;
        }

        /// <summary>
        /// Type of column
        /// </summary>
        public ColumnSchemaType SchemaType { get; set; }

        /// <summary>
        /// Full name with prefix and postfix
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
        /// Represents type name for column
        /// </summary>
        public SMType Type { get; }
    }
}