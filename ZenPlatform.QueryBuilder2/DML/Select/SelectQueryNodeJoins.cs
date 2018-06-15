using System;
using ZenPlatform.QueryBuilder2.From;

namespace ZenPlatform.QueryBuilder2.Select
{
    public partial class SelectQueryNode
    {
        public SelectQueryNode Join(JoinType joinType, string tableName,
            Action<TableNode> tableOptions, Action<JoinNode> joinOptions)
        {
            var tableNode = new TableNode(tableName);

            tableOptions(tableNode);

            var joinNode = new JoinNode(tableNode, joinType);

            joinOptions(joinNode);

            _from.Add(joinNode);
            return this;
        }

        public SelectQueryNode Join(JoinType joinType, SelectQueryNode queryNode,
            Action<SelectNastedQueryNode> nastedQueryOptions, Action<JoinNode> joinOptions
        )
        {
            var nastedQuery = new SelectNastedQueryNode(queryNode);

            nastedQueryOptions(nastedQuery);

            _from.Add(new JoinNode(nastedQuery, joinType));
            return this;
        }

        public SelectQueryNode Join(JoinType joinType, Action<SelectQueryNode> nastedQuery,
            Action<SelectNastedQueryNode> nastedQueryOptions, Action<JoinNode> joinOptions)
        {
            var queryNode = new SelectQueryNode();

            nastedQuery(queryNode);

            return Join(joinType, queryNode, nastedQueryOptions, joinOptions);
        }
    }
}