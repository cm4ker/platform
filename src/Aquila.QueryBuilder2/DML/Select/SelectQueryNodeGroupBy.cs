using System;
using System.Security.Cryptography.X509Certificates;
using Aquila.QueryBuilder.Common;
using Aquila.QueryBuilder.Common.Factoryes;

namespace Aquila.QueryBuilder.DML.Select
{
    public partial class SelectQueryNode
    {
        public SelectQueryNode GroupBy(string fieldName)
        {
            return GroupBy(x => x.Field(fieldName));
        }

        public SelectQueryNode GroupBy(Func<SqlNodeFactory, SqlNode> expression)
        {
            var factory = new SqlNodeFactory();
            _groupBy.Add(expression(factory));

            return this;
        }
    }
}