using System;
using Aquila.QueryBuilder.Common;
using Aquila.QueryBuilder.Common.Conditions;
using Aquila.Shared.Tree;

namespace Aquila.QueryBuilder.DML.Where
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