using System;
using System.Xml.Serialization;
using ZenPlatform.Configuration.Structure.Data.Types.Primitive;

namespace ZenPlatform.Configuration.Structure.Data.Types
{
    public abstract class XCTypeBase
    {
        protected XCTypeBase()
        {
        }

        /// <summary>
        /// Глобальный идентификатор типа, уникальный в разрезе конфигураций
        /// </summary>
        [XmlElement]
        public virtual Guid Guid { get; set; }

        /// <summary>
        /// Локальный уникальный идентификатор типа хранится в базе данных и присваивается инициализатороом во время загрузки конфигурации
        /// Если уникальность этого поля будет нарушена , то в таком случае это будет провал
        /// </summary>
        [XmlIgnore]
        public virtual uint Id { get; set; }

        /// <summary>
        /// Наименование типа
        /// </summary>
        [XmlElement]
        public virtual string Name { get; set; }

        /// <summary>
        /// Описание типа
        /// </summary>
        [XmlElement]
        public virtual string Description { get; set; }
    }

    public static class PlatformTypes
    {
        public static XCBinary Binary = new XCBinary();
        public static XCDateTime DateTime = new XCDateTime();
        public static XCString String = new XCString();
        public static XCBoolean Boolean = new XCBoolean();
        public static XCNumeric Numeric = new XCNumeric();
        public static XCGuid Guid = new XCGuid();
    }

    public class XCFakeType : XCTypeBase
    {
    }
}