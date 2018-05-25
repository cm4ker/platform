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
        [XmlAttribute] public Guid Id { get; set; }

        [XmlAttribute] public XCDateCaseType DateCase { get; set; }

        [XmlAttribute] public string Alias { get; set; }

        [XmlAttribute] public int Length { get; set; }

        [XmlAttribute] public int Precision { get; set; }

        [XmlAttribute] public bool Unique { get; set; }

        [XmlArray]
        [XmlArrayItem(ElementName = "Type")]
        public List<XCTypeBase> Types { get; }

        /// <summary>
        /// Колонка привязанная к базе данных
        /// </summary>
        [XmlAttribute]
        public string DatabaseColumnName { get; set; }
    }
}