using System;

namespace ZenPlatform.Configuration.Structure.Data.Types.Complex
{
    /// <summary>
    /// Описывает тип и название колонки
    /// </summary>
    public class XCColumnSchemaDefinition
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
}