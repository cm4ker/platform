using System;
using ZenPlatform.QueryBuilder.Common;
using ZenPlatform.QueryBuilder.Common.Conditions;
using ZenPlatform.Shared.Tree;

namespace ZenPlatform.QueryBuilder.DML.Where
{
    public class WhereNode : SqlNode
    {
        public WhereNode Where(Func<ConditionFactory, SqlNode> expr)
        {
            Childs.Add(expr(ConditionFactory.Get()));
            return this;
        }
    }
}