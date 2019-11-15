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
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public TNext AsAnsiString(string collationName)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public TNext AsAnsiString(int size)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public TNext AsAnsiString(int size, string collationName)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public TNext AsBinary()
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public TNext AsBinary(int size)
        {
            SetType(new ColumnTypeBynary() { Size = size });
            return (TNext)(object)this;
        }

        /// <inheritdoc />
        public TNext AsBoolean()
        {
            SetType(new ColumnTypeBool());
            return (TNext)(object)this;
        }

        /// <inheritdoc />
        public TNext AsByte()
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public TNext AsCurrency()
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public TNext AsDate()
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public TNext AsDateTime()
        {
            SetType(new ColumnTypeDataTime());
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
            SetType(new ColumnTypeDecimal());
            return (TNext)(object)this;
        }

        /// <inheritdoc />
        public TNext AsDecimal(int size, int precision)
        {
            SetType(new ColumnTypeDecimal() { Scale = size , Precision = precision});
            return (TNext)(object)this;
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
            SetType(new ColumnTypeFloat());
            return (TNext)(object)this;
        }

        /// <inheritdoc />
        public TNext AsGuid()
        {
            SetType(new ColumnTypeGuid());
            return (TNext)(object)this;
        }

        /// <inheritdoc />
        public TNext AsInt16()
        {
            SetType(new ColumnTypeSmallInt());
            return (TNext)(object)this;
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
            SetType(new ColumnTypeBigInt());
            return (TNext)(object)this;
        }

        /// <inheritdoc />
        public TNext AsString()
        {
            SetType(new ColumnTypeText());
            return (TNext)(object)this;
        }

        /// <inheritdoc />
        public TNext AsString(string collationName)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public TNext AsString(int size)
        {
            SetType(new ColumnTypeText() {  Size = size });
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
