using System;
using System.Collections.Generic;

namespace Aquila.Core.Querying.Model
{
    //Describe info about the type translated to the DB
    public class QTypeInfo : IEquatable<QTypeInfo>
    {
        public QTypeInfo(TypeKind kind)
        {
            Kind = kind;
        }

        public string Name { get; }

        public TypeKind Kind { get; }

        public bool IsObject => Kind == TypeKind.Link;

        public bool IsLink => Kind == TypeKind.Link;

        public bool IsPrimitive => (int)Kind < 4;

        public bool Equals(QTypeInfo other)
        {
            if (other == null) return false;

            return Name == other.Name && Kind == other.Kind;
        }

        public override bool Equals(object obj)
        {
            return Equals((QTypeInfo)obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name, (int)Kind);
        }
    }

    public enum TypeKind
    {
        Numeric = 1,
        String = 2,
        Binary = 3,
        Date = 4,
        Boolean = 5,
        Int = 6,
        Link = 7,
        Unknown = 0
    }
}