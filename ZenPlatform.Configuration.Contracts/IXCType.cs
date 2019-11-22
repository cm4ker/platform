using System;
using System.Xml.Serialization;

namespace ZenPlatform.Configuration.Contracts
{
    public interface IXCType : IEquatable<IXCType>
    {
        /// <summary>
        /// Глобальный идентификатор типа, уникальный в разрезе конфигураций
        /// </summary>
        [XmlElement]
        Guid Guid { get; set; }

        /// <summary>
        /// Локальный уникальный идентификатор типа хранится в базе данных и присваивается инициализатороом во время загрузки конфигурации
        /// Если уникальность этого поля будет нарушена , то в таком случае это будет провал
        /// </summary>
        [XmlIgnore]
        uint Id { get; set; }

        /// <summary>
        /// Наименование типа
        /// </summary>
        [XmlElement]
        string Name { get; set; }

        /// <summary>
        /// Описание типа
        /// </summary>
        [XmlElement]
        string Description { get; set; }


        bool IsAssignableFrom(IXCType tb);
    }
}