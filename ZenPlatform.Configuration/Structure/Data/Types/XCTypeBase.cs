using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Xml.Serialization;

namespace ZenPlatform.Configuration.Structure.Data.Types
{
    public abstract class XCTypeBase : IEquatable<XCTypeBase>
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

        public virtual bool IsAssignableFrom(XCTypeBase tb)
        {
            return tb.Guid == this.Guid;
        }

        public virtual bool Equals(XCTypeBase other)
        {
            return Guid.Equals(other?.Guid) && Id == other?.Id;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((XCTypeBase) obj);
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


    public class XCTypeBaseEqualityComparer : IEqualityComparer<XCTypeBase>
    {
        public bool Equals([AllowNull] XCTypeBase x, [AllowNull] XCTypeBase y)
        {
            if (x == null && y == null)
                return true;
            else if (x == null || y == null)
                return false;
            return x.Guid == y.Guid;
        }

        public int GetHashCode([DisallowNull] XCTypeBase obj)
        {
            return obj.Guid.GetHashCode();
        }
    }
}