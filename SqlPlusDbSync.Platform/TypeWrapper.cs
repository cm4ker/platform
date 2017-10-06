using System;
using Newtonsoft.Json;

namespace SqlPlusDbSync.Platform
{
    public sealed class TypeWrapper<T> : TypeWrapper
    {
        public TypeWrapper() : base() { }

        public TypeWrapper(T value)
            : base()
        {
            this.Value = value;
        }

        public override object ObjectValue { get { return Value; } }

        public T Value { get; set; }
    }

    public abstract class TypeWrapper
    {
        protected TypeWrapper() { }

        [JsonIgnore]
        public abstract object ObjectValue { get; }

        public static TypeWrapper CreateWrapper<T>(T value)
        {
            if (value == null)
                return new TypeWrapper<T>();
            var type = value.GetType();
            if (type == typeof(T))
                return new TypeWrapper<T>(value);
            // Return actual type of subclass
            return (TypeWrapper)Activator.CreateInstance(typeof(TypeWrapper<>).MakeGenericType(type), value);
        }

        public TypeCode GetTypeCode()
        {
            return Convert.GetTypeCode(ObjectValue);
        }

        public bool ToBoolean(IFormatProvider provider)
        {
            return (bool)ObjectValue;
        }

        public char ToChar(IFormatProvider provider)
        {
            return (char)ObjectValue;
        }

        public sbyte ToSByte(IFormatProvider provider)
        {
            return (sbyte)ObjectValue;
        }

        public byte ToByte(IFormatProvider provider)
        {
            return (byte)ObjectValue;
        }

        public short ToInt16(IFormatProvider provider)
        {
            return (short)ObjectValue;
        }

        public ushort ToUInt16(IFormatProvider provider)
        {
            return (ushort)ObjectValue;
        }

        public int ToInt32(IFormatProvider provider)
        {
            return (int)ObjectValue;
        }

        public uint ToUInt32(IFormatProvider provider)
        {
            return (uint)ObjectValue;
        }

        public long ToInt64(IFormatProvider provider)
        {
            return (long)ObjectValue;
        }

        public ulong ToUInt64(IFormatProvider provider)
        {
            return (ulong)ObjectValue;
        }

        public float ToSingle(IFormatProvider provider)
        {
            return (float)ObjectValue;
        }

        public double ToDouble(IFormatProvider provider)
        {
            return (double)ObjectValue;
        }

        public decimal ToDecimal(IFormatProvider provider)
        {
            return (decimal)ObjectValue;
        }

        public DateTime ToDateTime(IFormatProvider provider)
        {
            return (DateTime)ObjectValue;
        }

        public string ToString(IFormatProvider provider)
        {
            return ObjectValue.ToString();
        }

        public object ToType(Type conversionType, IFormatProvider provider)
        {
            return ObjectValue;
        }
    }
}