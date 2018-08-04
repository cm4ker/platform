using System;
using ZenPlatform.QueryBuilder.Common.Factoryes;
using ZenPlatform.QueryBuilder.Common.Table;
using ZenPlatform.QueryBuilder.DML.Select;
using ZenPlatform.Shared.Tree;

namespace ZenPlatform.QueryBuilder.DML.Insert
{
    public class InsertQueryNode : SqlNode, IInsertQuery
    {
        private TableWithColumnsNode _table;
        private InsertValuesNode _values;


        public InsertQueryNode()
        {
            _values = new InsertValuesNode();
        }

        public InsertQueryNode InsertInto(string tableName)
        {
            return InsertInto(f => f.InsertTable(tableName));
        }

        public InsertQueryNode InsertInto(string schemaName, string tableName)
        {
            return InsertInto(f => f.InsertTable(tableName).WithSchema(schemaName));
        }

        public InsertQueryNode InsertInto(Func<SqlNodeFactory, TableWithColumnsNode> exp)
        {
            var fac = new SqlNodeFactory();
            _table = exp(fac);
            return this;
        }

        public InsertQueryNode WithField(Func<SqlNodeFactory, FieldNode> fieldExp)
        {
            var fac = new SqlNodeFactory();
            _table.WithField(fieldExp(fac));
            //_insertInto.Add(exp(fac));
            return this;
        }

        public InsertQueryNode WithValue(Func<SqlNodeFactory, SqlNode> valExp)
        {
            var fac = new SqlNodeFactory();
            _values.Add(valExp(fac));
            return this;
        }

        public InsertQueryNode WithFieldAndValue(Func<SqlNodeFactory, FieldNode> fieldExp,
            Func<SqlNodeFactory, SqlNode> valExp)
        {
            WithField(fieldExp);
            return WithValue(valExp);
        }

        TableWithColumnsNode IInsertQuery.TableWithColumnsNode => _table;
        InsertValuesNode IInsertQuery.InsertValuesNode => _values;
    }

    public interface IInsertQuery
    {
        TableWithColumnsNode TableWithColumnsNode { get; }
        InsertValuesNode InsertValuesNode { get; }
    }
}