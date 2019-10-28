using ZenPlatform.QueryBuilder.Contracts;
using System;
using System.Collections.Generic;
using System.Text;
using ZenPlatform.QueryBuilder.Common.Columns;
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
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public TNext AsAnsiString(string collationName)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public TNext AsAnsiString(int size)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public TNext AsAnsiString(int size, string collationName)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public TNext AsBinary()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public TNext AsBinary(int size)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public TNext AsBoolean()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public TNext AsByte()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public TNext AsCurrency()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public TNext AsDate()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public TNext AsDateTime()
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public TNext AsGuid()
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public TNext AsString(string collationName)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public TNext AsString(int size)
        {
            throw new NotImplementedException();
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
