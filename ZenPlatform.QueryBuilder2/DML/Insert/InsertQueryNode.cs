using System;
using ZenPlatform.QueryBuilder2.Common;
using ZenPlatform.QueryBuilder2.Common.Factoryes;
using ZenPlatform.QueryBuilder2.Common.Table;
using ZenPlatform.QueryBuilder2.DML.From;
using ZenPlatform.QueryBuilder2.DML.Select;
using ZenPlatform.Shared.Tree;

namespace ZenPlatform.QueryBuilder2.DML.Insert
{
    public class InsertQueryNode : SqlNode
    {
        private InsertIntoNode _insertInto;
        private TableWithColumnsNode _table;
        private InsertValuesNode _values;

        public InsertQueryNode()
        {
            _insertInto = new InsertIntoNode();
            _values = new InsertValuesNode();
            Childs.AddRange(new SqlNode[] {_insertInto, _values});
        }

        public InsertQueryNode InsertInto(string tableName)
        {
            return InsertInto(f => f.InsertTable(tableName));
        }

        public InsertQueryNode InsertInto(string schemaName, string tableName)
        {
            return InsertInto(f => f.InsertTable(tableName).WithSchema(schemaName));
        }

        public InsertQueryNode InsertInto(Func<NodeFactory, TableWithColumnsNode> exp)
        {
            var fac = new NodeFactory();
            //_insertInto.Add(exp(fac));
            _table = exp(fac);
            Childs.Insert(1, _table);
            return this;
        }

        public InsertQueryNode WithField(Func<NodeFactory, FieldNode> fieldExp)
        {
            var fac = new NodeFactory();
            _table.WithField(fieldExp(fac));
            //_insertInto.Add(exp(fac));
            return this;
        }

        public InsertQueryNode WithValue(Func<NodeFactory, SqlNode> valExp)
        {
            var fac = new NodeFactory();
            _values.Add(valExp(fac));
            return this;
        }

        public InsertQueryNode WithFieldAndValue(Func<NodeFactory, FieldNode> fieldExp,
            Func<NodeFactory, SqlNode> valExp)
        {
            WithField(fieldExp);
            return WithValue(valExp);
        }

        /*
         *
         * INSERT INTO Table(a,b,c,e) VALUES() 
         * 
         */
    }
}