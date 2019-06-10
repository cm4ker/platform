using System;
using System.Collections.Generic;
using System.Linq;

namespace ZenPlatform.AsmInfrastructure
{
    /// <summary>
    /// Структура для мультитипового хранения данных
    /// </summary>
    public struct MultiDataStorage : IEquatable<MultiDataStorage>
    {
        private readonly MultiType _multiType;

        public MultiDataStorage(MultiType multiType, object value)
        {
            _multiType = multiType;
            if (_multiType.Check(value.GetType()))
                _value = value;
            else
                throw new Exception("Can't assign this value to this type");
        }

        private object _value;

        private object Value
        {
            get => _value;
            set
            {
                if (!Check(value))
                    throw new Exception("You can't assign this type of value here");

                _value = value;
            }
        }

        public bool Check(object value)
        {
            return Check(value.GetType());
        }


        public T GetValue<T>()
        {
            if (!_multiType.Check<T>()) throw new Exception("Not allowed type here");

            return (T) Value;
        }


        public bool Equals(MultiDataStorage other)
        {
            return Value == other.Value;
        }

        public override bool Equals(object obj)
        {
            return obj is MultiDataStorage other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (_value != null ? _value.GetHashCode() : 0);
            }
        }
    }

    /// <summary>
    /// Мультитип
    /// </summary>
    public struct MultiType : IEquatable<MultiType>
    {
        public MultiType(IEnumerable<Type> types)
        {
            Types = types;
        }

        private readonly IEnumerable<Type> Types;

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