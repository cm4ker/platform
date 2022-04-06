using System;
using Aquila.Core.Querying;
using Aquila.Data;
using Aquila.QueryBuilder.Model;
using Aquila.QueryBuilder.Visitor;

namespace Aquila.QueryBuilder
{
    public class SqlCompiler
    {
        private readonly QueryVisitorBase<string> _visitor;

        private SqlCompiler(QueryVisitorBase<string> visitor)
        {
            _visitor = visitor;
        }

        public static SqlCompiler FormEnum(SqlDatabaseType dbType)
        {
            QueryVisitorBase<string> builder = dbType switch
            {
                SqlDatabaseType.SqlServer => new MsSqlBuilder(),
                SqlDatabaseType.Postgres => new PostgresBuilder(),
                _ => throw new NotImplementedException("Not implemented")
            };

            return new SqlCompiler(builder);
        }

        public string Compile(SSyntaxNode node)
        {
            return _visitor.Visit(node);
        }

        public string Compile(QueryMachine queryMachine)
        {
            return _visitor.Visit((SSyntaxNode)queryMachine.peek());
        }
    }
}