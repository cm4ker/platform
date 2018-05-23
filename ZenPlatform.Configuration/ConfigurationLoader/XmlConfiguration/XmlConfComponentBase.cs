using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace ZenPlatform.Configuration.ConfigurationLoader.XmlConfiguration
{
    public abstract class XmlConfComponentBase
    {
        [XmlElement]
        public Guid Id { get; set; }

        [XmlElement]
        public string Name { get; set; }

        [XmlElement]
        public bool IsAbstract { get; set; }

        [XmlElement]
        public bool IsSealed { get; set; }

        [XmlAttribute]
        public Guid BaseTypeId { get; set; }
    }

    public abstract class XmlConfComponentPropertyBase
    {
        [XmlAttribute]
        public Guid Id { get; set; }

        [XmlAttribute]
        public Guid TypeId { get; set; }

        [XmlAttribute]
        public XmlConfDateCaseType DateCase { get; set; }

        [XmlAttribute]
        public string Alias { get; set; }

        [XmlAttribute]
        public int Length { get; set; }

        [XmlAttribute]
        public int Precision { get; set; }

        /// <summary>
        /// Колонка привязанная к базе данных
        /// </summary>
        [XmlAttribute]
        public string DatabaseColumnName { get; set; }
    }


    public enum XmlConfDateCaseType
    {
        [XmlEnum("DateTime")]
        DateTime,

        [XmlEnum("Date")]
        Date,

        [XmlEnum("Time")]
        Time,

    }
}
