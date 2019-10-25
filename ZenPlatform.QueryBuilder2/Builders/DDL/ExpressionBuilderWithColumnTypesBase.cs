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
        public abstract ColumnDefinitionNode GetColumnForType();

        /// <summary>
        /// Gets the current column definition
        /// </summary>
        private ColumnDefinitionNode Column => GetColumnForType();

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
            Column.Type = new VarbinaryTypeDefinitionNode(0);

            return (TNext)(object)this;
        }

        /// <inheritdoc />
        public TNext AsBinary(int size)
        {
            Column.Type = new VarbinaryTypeDefinitionNode(size);
            return (TNext)(object)this;
        }

        /// <inheritdoc />
        public TNext AsBoolean()
        {
            Column.Type = new BooleanTypeDefinitionNode();
            return (TNext)(object)this;
        }

        /// <inheritdoc />
        public TNext AsByte()
        {
            Column.Type = new ByteTypeDefinitionNode();
            return (TNext)(object)this;
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
            Column.Type = new DateTimeTypeDefinitionNode();
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
            Column.Type = new DoubleTypeDefinitionNode();
            return (TNext)(object)this;
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
            Column.Type = new FloatTypeDefinitionNode();
            return (TNext)(object)this;
        }

        /// <inheritdoc />
        public TNext AsGuid()
        {
            Column.Type = new GuidTypeDefinitionNode();
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
            Column.Type = new IntTypeDefinitionNode();
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
            Column.Type = new TextTypeDefinitionNode();
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
            Column.Type = new XmlTypeDefinitionNode();
            return (TNext)(object)this;
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
