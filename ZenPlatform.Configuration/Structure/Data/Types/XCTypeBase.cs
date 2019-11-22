using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Xml.Serialization;
using ZenPlatform.Configuration.Contracts;
using ZenPlatform.Configuration.Structure.Data.Types.Complex;

namespace ZenPlatform.Configuration.Structure.Data.Types
{
    public abstract class XCTypeBase : IXCType
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

        public virtual bool IsAssignableFrom(IXCType tb)
        {
            return tb.Guid == this.Guid;
        }

        public virtual bool Equals(IXCType other)
        {
            return Guid.Equals(other?.Guid) && Id == other?.Id;
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
            unchecked
            {
                var hashCode = Guid.GetHashCode();
                hashCode = (hashCode * 397) ^ (int) Id;
                hashCode = (hashCode * 397) ^ (Name != null ? Name.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Description != null ? Description.GetHashCode() : 0);
                return hashCode;
            }
        }
    }


    public class XCTypeBaseEqualityComparer : IEqualityComparer<IXCType>
    {
        public bool Equals([AllowNull] IXCType x, [AllowNull] IXCType y)
        {
            if (x == null && y == null)
                return true;
            else if (x == null || y == null)
                return false;
            return x.Guid == y.Guid;
        }

        public int GetHashCode([DisallowNull] IXCType obj)
        {
            return obj.Guid.GetHashCode();
        }
    }
}