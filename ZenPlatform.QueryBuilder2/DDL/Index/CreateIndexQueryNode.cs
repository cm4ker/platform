using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using ZenPlatform.QueryBuilder.Common;
using ZenPlatform.QueryBuilder.Common.Columns;
using ZenPlatform.QueryBuilder.Common.SqlTokens;
using ZenPlatform.QueryBuilder.Common.Table;
using ZenPlatform.Shared.ParenChildCollection;
using ZenPlatform.Shared.Tree;

namespace ZenPlatform.QueryBuilder.DDL.Index
{
    /// <summary>
    /// Построитель запроса "Создать новый индекс"
    /// </summary>
    public class CreateIndexQueryNode : SqlNode, ICreateIndexQuery
    {
        private SqlNode _indexName;
        private IndexTableNode _table;

        public CreateIndexQueryNode(string indexName)
        {
            _indexName = new IdentifierNode(indexName);
        }

        public CreateIndexQueryNode WithTable(string tableName, Action<IndexTableNode> options)
        {
            _table = new IndexTableNode(tableName);
            options?.Invoke(_table);
            return this;
        }

        SqlNode ICreateIndexQuery.IndexName => _indexName;
        IndexTableNode ICreateIndexQuery.TargetTable => _table;
    }

    /// <summary>
    /// Представляет собой таблицу + описание полей в ней
    /// </summary>
    public class IndexTableNode : SqlNode
    {
        private TableNode _table;
        private ColumnListNode _columns;

        public IndexTableNode(string tableName)
        {
            _table = new TableNode(tableName);
            _columns = new ColumnListNode();

            Childs.AddRange(_table, _columns);
        }

        public IndexTableNode WithSchema(string schemaName)
        {
            _table.WithSchema(schemaName);
            return this;
        }

        public IndexTableNode WithColumn(string name, Action<IndexTableColumnNode> options)
        {
            var newColumn = new IndexTableColumnNode(name);
            options?.Invoke(newColumn);

            _columns.Add(newColumn);

            return this;
        }

        public TableNode Table => _table;
        public ColumnListNode ColumnList => _columns;
    }

    public class IndexTableColumnNode : SqlNode
    {
        public IndexTableColumnNode(string name)
        {
            Childs.Add(new IdentifierNode(name));
            Direction = Tokens.AscToken;
        }

        public void WithDirection(ListSortDirection direction)
        {
            if (direction == ListSortDirection.Ascending)
                Direction = Tokens.AscToken;
            else
                Direction = Tokens.DescToken;
        }

        public SqlNode Direction { get; private set; }
    }

    public interface ICreateIndexQuery : IChildItem<Node>, IParentItem<Node, Node>
    {
        SqlNode IndexName { get; }
        IndexTableNode TargetTable { get; }
    }
}
