using System;
using System.Xml.Serialization;
using ZenPlatform.Configuration.Contracts;

namespace ZenPlatform.Configuration.Common
{
    public abstract class TypeBase : IXCType
    {
        protected TypeBase()
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

        public virtual bool IsAssignableFrom(IXCType tb)
        {
            return tb.Guid == this.Guid;
        }

        public virtual bool Equals(IXCType other)
        {
            return Guid.Equals(other?.Guid);//&& Id == other?.Id;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((IXCType) obj);
        }

        public override int GetHashCode()
        {
            return Guid.GetHashCode();
        }
    }

}