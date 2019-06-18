using System;
using System.Collections.Generic;
using System.Linq;

namespace ZenPlatform.Compiler.Infrastructure
{
    /// <summary>
    /// Мультитип
    /// </summary>
    public struct MultiType : IEquatable<MultiType>
    {
        public MultiType(IEnumerable<Type> types)
        {
            Types = types;
        }

        public MultiType(Type[] types)
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

        public bool Equals(MultiType other)
        {
            return Equals(Types, other.Types);
        }

        public override bool Equals(object obj)
        {
            return obj is MultiType other && Equals(other);
        }

        public override int GetHashCode()
        {
            return (Types != null ? Types.GetHashCode() : 0);
        }
    }
}