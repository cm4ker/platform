using ZenPlatform.QueryBuilder.Contracts;
using System;
using System.Collections.Generic;
using System.Text;
using ZenPlatform.QueryBuilder.Model;

namespace ZenPlatform.QueryBuilder.Builders
{
    public abstract class ExpressionBuilderWithColumnTypesBase<TNext> : IColumnTypeSyntax<TNext>
    {

        protected ExpressionBuilderWithColumnTypesBase()
        {
        }

        /// <summary>
        /// Returns the column definition to set the type for
        /// </summary>
        /// <returns>The column definition to set the type for</returns>
        public abstract void SetType(ColumnType columnType);


        /// <inheritdoc />
        public TNext AsAnsiString()
        {
            SetType(new ColumnTypeInt());
            return (TNext)(object)this;
        }

        /// <inheritdoc />
        public TNext AsAnsiString(string collationName)
        {
            SetType(new ColumnTypeInt());
            return (TNext)(object)this;
        }

        /// <inheritdoc />
        public TNext AsAnsiString(int size)
        {
            SetType(new ColumnTypeInt());
            return (TNext)(object)this;
        }

        /// <inheritdoc />
        public TNext AsAnsiString(int size, string collationName)
        {
            SetType(new ColumnTypeInt());
            return (TNext)(object)this;
        }

        /// <inheritdoc />
        public TNext AsBinary()
        {
            SetType(new ColumnTypeInt());
            return (TNext)(object)this;
        }

        /// <inheritdoc />
        public TNext AsBinary(int size)
        {
            SetType(new ColumnTypeInt());
            return (TNext)(object)this;
        }

        /// <inheritdoc />
        public TNext AsBoolean()
        {
            SetType(new ColumnTypeInt());
            return (TNext)(object)this;
        }

        /// <inheritdoc />
        public TNext AsByte()
        {
            SetType(new ColumnTypeInt());
            return (TNext)(object)this;
        }

        /// <inheritdoc />
        public TNext AsCurrency()
        {
            SetType(new ColumnTypeInt());
            return (TNext)(object)this;
        }

        /// <inheritdoc />
        public TNext AsDate()
        {
            SetType(new ColumnTypeInt());
            return (TNext)(object)this;
        }

        /// <inheritdoc />
        public TNext AsDateTime()
        {
            SetType(new ColumnTypeInt());
            return (TNext)(object)this;
        }

        /// <inheritdoc />
        public TNext AsDateTime2()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public TNext AsDateTimeOffset()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public TNext AsDateTimeOffset(int precision)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public TNext AsDecimal()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public TNext AsDecimal(int size, int precision)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public TNext AsDouble()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public TNext AsFixedLengthString(int size)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public TNext AsFixedLengthString(int size, string collationName)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public TNext AsFixedLengthAnsiString(int size)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public TNext AsFixedLengthAnsiString(int size, string collationName)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public TNext AsFloat()
        {
            SetType(new ColumnTypeInt());
            return (TNext)(object)this;
        }

        /// <inheritdoc />
        public TNext AsGuid()
        {
            SetType(new ColumnTypeInt());
            return (TNext)(object)this;
        }

        /// <inheritdoc />
        public TNext AsInt16()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public TNext AsInt32()
        {
            SetType(new ColumnTypeInt());
            return (TNext)(object)this;
        }

        /// <inheritdoc />
        public TNext AsInt64()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public TNext AsString()
        {
            SetType(new ColumnTypeInt());
            return (TNext)(object)this;
        }

        /// <inheritdoc />
        public TNext AsString(string collationName)
        {
            SetType(new ColumnTypeInt());
            return (TNext)(object)this;
        }

        /// <inheritdoc />
        public TNext AsString(int size)
        {
            SetType(new ColumnTypeInt());
            return (TNext)(object)this;
        }

        /// <inheritdoc />
        public TNext AsString(int size, string collationName)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public TNext AsTime()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public TNext AsXml()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public TNext AsXml(int size)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public TNext AsCustom(string customType)
        {
            throw new NotImplementedException();
        }


    }
}
