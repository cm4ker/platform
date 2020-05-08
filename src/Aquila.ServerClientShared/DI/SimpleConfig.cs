using System;
using System.Collections.Generic;
using System.Text;

namespace Aquila.Core.DI
{
    public class SimpleConfig<T> : IConfig<T> where T : class
    {
        public static SimpleConfig<TValue> Instance<TValue>(TValue value) where TValue : class
        {
            return new SimpleConfig<TValue>(value);
        }

        public SimpleConfig(T value)
        {
            Value = value;
        }

        public T Value { get; private set; }
    }
}
