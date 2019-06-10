using System;
using System.Collections.Generic;
using System.Linq;

namespace ZenPlatform.Compiler.Infrastructure
{
    /*
     * МУЛЬТИТИПЫ
     * Представление:
     *     Для мультитипизации характерны следующие кейсы:
     *         1) У нас есть свойство в котором может содержаться несколько типов данных
     *         2) Мы хотим чтобы обрабатываемый нами объект имел конечное количество типов (в C# это достигается
     *         через конструкцию Generic<T> : where T:RealTypeInterfaceOrSomethingElse )
     * Мультитипы представлены в языке следующей конструкцией: <type [, type]> 
     *
     * Пример:
     * <int, string, double, char, Invoice> 
     *
     * Мультитипизация доступна как на серверной части, так и на клиентской
     *
     * Вычисление типа происходит до компилирования, после верефицирования кода
     *
     * Мультитипы МОГУТ передаваться с клиента на сервер и с сервера на клиент
     *
     * Мультитип содержит в себе список поддерживаемых типов.
     *
     * Мультитип можно объявить отдельно : set of type MultitypeName = <type [, type]> 
     *
     * Пример:
     *
     * set of type MyMultitype = <int, string>; 
     *
     * Мультитип можно вложить в другой мультитип 
     *
     * Пример:
     * set of type MyMultitype2 = <MyMultitype, char>
     *
     * При этом произойдёт слияние типов, т.е. Multitype2 = <int, string, char>
     *
     * Мультитипы необходимо привести перед тем как пытаться выполнить какие-либо операции
     *     Mt1 t1; Mt2 t2;
     *
     *     var result = (int)t1 + (int)t2
     *
     * У мультитипа доступен pattern matching
     *
     *     <string, int, char> t1;
     *
     *     match t1:
     *     | string & char => {Statements block} // Это строка или символ
     *     | int => {Statements block} // это число
     *     | default => {statements block} // Мы не поняли что в выражении
     *     
     */
    
    /// <summary>
    /// Структура для мультитипового хранения данных
    /// </summary>
    public struct MultiTypeDataStorage : IEquatable<MultiTypeDataStorage>
    {
        private readonly MultiType _multiType;

        public MultiTypeDataStorage(MultiType multiType, object value)
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


        public bool Equals(MultiTypeDataStorage other)
        {
            return Value == other.Value;
        }

        public override bool Equals(object obj)
        {
            return obj is MultiTypeDataStorage other && Equals(other);
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