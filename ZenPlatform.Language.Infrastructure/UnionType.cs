using System;
using System.Collections.Generic;
using System.Linq;

namespace ZenPlatform.Compiler.Infrastructure
{
    /// <summary>
    /// Мультитип
    /// </summary>
    public struct UnionType : IEquatable<UnionType>
    {
        public UnionType(IEnumerable<Type> types)
        {
            Types = types;
        }

        public UnionType(Type[] types)
        {
            Types = types;
        }

        public readonly IEnumerable<Type> Types;

        public bool Check(Type type)
        {
            return Types.Contains(type);
        }

        public bool Check<T>()
        {
            return Check(typeof(T));
        }

        public bool Equals(UnionType other)
        {
            return Equals(Types, other.Types);
        }

        public override bool Equals(object obj)
        {
            return obj is UnionType other && Equals(other);
        }

        public override int GetHashCode()
        {
            return (Types != null ? Types.GetHashCode() : 0);
        }
    }
}