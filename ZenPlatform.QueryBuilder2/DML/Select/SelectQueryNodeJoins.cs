using System;
using ZenPlatform.QueryBuilder.DML.From;

namespace ZenPlatform.QueryBuilder.DML.Select
{
    public partial class SelectQueryNode
    {
        public SelectQueryNode Join(JoinType joinType, string tableName,
            Action<AliasedTableNode> tableOptions, Action<JoinNode> joinOptions)
        {
            var tableNode = new AliasedTableNode(tableName);

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