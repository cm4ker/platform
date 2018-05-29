using System;
using System.Xml.Serialization;

namespace ZenPlatform.Configuration.ConfigurationLoader.XmlConfiguration.Data.Types
{
    public abstract class XCTypeBase
    {
        protected XCTypeBase()
        {
        }


        /// <summary>
        /// Уникальный идентификатор типа
        /// </summary>
        [XmlElement]
        public virtual Guid Id { get; set; }

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
}