using System;
using System.Xml.Serialization;

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

        
        protected virtual bool ShouldSerializeName()
        {
            return true;
        }
        
        protected virtual bool ShouldSerializeId()
        {
            return true;
        }
        
        protected virtual bool ShouldSerializeDescription()
        {
            return true;
        }
    }
}