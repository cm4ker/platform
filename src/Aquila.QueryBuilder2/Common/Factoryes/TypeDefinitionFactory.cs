using Aquila.QueryBuilder.Common.Operations;
using Aquila.QueryBuilder.DML.From;

namespace Aquila.QueryBuilder.Common.Factoryes
{
    public class TypeDefinitionFactory
    {
        /// <summary>
        /// Varchar type
        /// </summary>
        /// <param name="size">Use 0 for MAX size</param>
        /// <returns></returns>
        public TypeDefinitionNode Varchar(int size)
        {
            return new VarcharTypeDefinitionNode(size);
        }

        public TypeDefinitionNode DateTime()
        {
            return new DateTimeTypeDefinitionNode();
        }

        public TypeDefinitionNode Varbinary(int size)
        {
            return new VarbinaryTypeDefinitionNode(size);
        }

        public TypeDefinitionNode Boolean()
        {
            return new BooleanTypeDefinitionNode();
        }

        public TypeDefinitionNode Int()
        {
            return new IntTypeDefinitionNode();
        }

        public TypeDefinitionNode Bigint()
        {
            return new BigintTypeDefinitionNode();
        }

        public TypeDefinitionNode Numeric(int scale, int precision)
        {
            return new NumericTypeDefinitionNode(scale, precision);
        }

        public TypeDefinitionNode Guid()
        {
            return new GuidTypeDefinitionNode();
        }
    }
}