using System;
using System.Collections;
using System.Text;

namespace Aquila.QueryBuilder
{
    /// <summary>
    /// This is string constant in query.
    /// Care this class haven't escaping deny symbols
    /// </summary>
    public class DBConstant : DBClause
    {
        private object _value;

        public DBConstant(object value)
        {
            _value = value;
        }

        protected object Value
        {
            get { return _value; }
        }

        public override string Compile(bool recompile = false)
        {
            if (_value is null)
                return SQLTokens.NULL;

            if (_value is int)
                return $"{_value}";
            return $"'{_value}'";
        }
    }

    public class DBHexConstant : DBConstant
    {
        public DBHexConstant(object value) : base(value)
        {
            if (!(value is int) && value != null)
            {
                throw new Exception("can't convert it into hex");
            }
        }

        public override string Compile(bool recompile = false)
        {
            if (Value is null)
                return "0x";
            return string.Format("0x{0:X}", Value);
        }
    }

    public class DBArrayConstant : DBClause
    {
        private readonly object _value;

        public DBArrayConstant(object value)
        {
            _value = value;
        }

        public override string Compile(bool recompile = false)
        {
            var sb = new StringBuilder();
            var arr = _value as IEnumerable ?? throw new Exception("Value is not array!");
            var index = 0;
            sb.Append("(");
            foreach (var item in arr)
            {
                if (index > 0)
                    sb.Append(",");

                sb.AppendFormat("'{0}'", item);

                index++;
            }
            sb.Append(")");
            return sb.ToString();
        }
    }
}