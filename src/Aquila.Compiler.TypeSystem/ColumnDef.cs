using System;
using Aquila.Compiler.Aqua.TypeSystem;

namespace Aquila.Compiler.Aqua
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
        public ColumnSchemaDefinition(ColumnSchemaType schemaType, PType platformIpType, string name,
            string prefix = "", string postfix = "")
        {
            SchemaType = schemaType;
            Name = name;
            PlatformIpType = platformIpType;
            Prefix = prefix ?? throw new ArgumentNullException();
            Postfix = postfix ?? throw new ArgumentNullException();
        }

        /// <summary>
        /// Тип колонки
        /// </summary>
        public ColumnSchemaType SchemaType { get; set; }

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
        public PType PlatformIpType { get; set; }
    }
}