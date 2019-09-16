using System;

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
     *     match t1.Type:
     *     | string & char => {Statements block} // Это строка или символ
     *     | int => {Statements block} // это число
     *     | default => {statements block} // Мы не поняли что в выражении
     *
     *
     * По идее у нас есть право на объявление вот такого вот свойства
     *
     * Property: <Invoice, CashOrder, NomenclatureReference> SomeProperty { get; set; } 
     *
     * в компилированном коде это должно выглядить
     *
     * UnionTypeStorage SomeProperty { get; set; }
     *
     * На самом деле может быть такая штука
     *
     * public class UnionType0001
     * {
     *    Invoice _Invoice;
     *    CashOrder _CashOrder;
     *    NomenclatureReference _NomenclatureReference;
     *
     *    public object Get()
     *    {
     *        return _Invoice ?? _CashOrder ?? _NomenclatureReference;
     *    }
     *
     *    public Set(object type)
     *    {
     *        
     *    }
     * }
     *     
     */

    /// <summary>
    /// Структура для мультитипового хранения данных
    /// </summary>
    public struct UnionTypeStorage : IEquatable<UnionTypeStorage>
    {
        private readonly UnionType _unionType;

        public UnionTypeStorage(object value, UnionType unionType)
        {
            _unionType = unionType;
            if (_unionType.Check(value.GetType()))
                _value = value;
            else
                throw new Exception("Can't assign this value to this type");
        }

        public UnionTypeStorage(UnionType unionType) : this(null, unionType)
        {
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
            if (!_unionType.Check<T>()) throw new Exception("Not allowed type here");

            return (T) Value;
        }


        public bool Equals(UnionTypeStorage other)
        {
            return Value == other.Value;
        }

        public override bool Equals(object obj)
        {
            return obj is UnionTypeStorage other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (_value != null ? _value.GetHashCode() : 0);
            }
        }
    }
}