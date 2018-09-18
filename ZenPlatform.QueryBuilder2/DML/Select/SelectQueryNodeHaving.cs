using System;
using ZenPlatform.QueryBuilder.Common;
using ZenPlatform.QueryBuilder.Common.Conditions;
using ZenPlatform.QueryBuilder.Common.Factoryes;
using ZenPlatform.QueryBuilder.DML.Where;

namespace ZenPlatform.QueryBuilder.DML.Select
{
    public partial class SelectQueryNode
    {
        #region Where

        public SelectQueryNode Having(string rawLeft, string operation, string rawRight)
        {
            _where.Add(new BinaryConditionNode(rawLeft, operation, rawRight));
            return this;
        }

        public SelectQueryNode Having(Func<SqlNodeFactory, SqlNode> f1, string operation,
            Func<SqlNodeFactory, SqlNode> f2)
        {
            var factory = new SqlNodeFactory();
            _where.Add(new BinaryConditionNode(f1(factory), operation, f2(factory)));
            return this;
        }

        public SelectQueryNode HavingIsNull(Func<SqlNodeFactory, SqlNode> fieldExp)
        {
            var factory = new SqlNodeFactory();
            _where.Add(new IsNullWhereNode(fieldExp(factory)));
            return this;
        }

        public SelectQueryNode HavingLike(Func<SqlNodeFactory, SqlNode> fieldExp, string pattern)
        {
            var factory = new SqlNodeFactory();
            _where.Add(new LikeConditionNode(fieldExp(factory), new StringLiteralNode(pattern)));
            return this;
        }

        public SelectQueryNode HavingIn(Func<SqlNodeFactory, SqlNode> fieldExp, Func<SqlNodeFactory, SqlNode> fieldExp2)
        {
            var factory = new SqlNodeFactory();
            _where.Add(new InConditionNode(fieldExp(factory), fieldExp2(factory)));
            return this;
        }

        #endregion
    }
}