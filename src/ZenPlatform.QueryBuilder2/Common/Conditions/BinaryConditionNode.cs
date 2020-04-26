using ZenPlatform.QueryBuilder.Common.SqlTokens;

namespace ZenPlatform.QueryBuilder.Common.Conditions
{
    public class BinaryConditionNode : ConditionNode
    {
        public SqlNode Left { get; }
        public SqlNode Operation { get; }
        public SqlNode Right { get; }

        public BinaryConditionNode(string rawLeft, string operation, string rawRight)
        {
            Left = new RawSqlNode(rawLeft);
            Operation = new RawSqlNode(operation);
            Right = new RawSqlNode(rawRight);

            Childs.AddRange(Left, Operation, Right);
        }

        public BinaryConditionNode(SqlNode node1, string operation, SqlNode node2)
        {
            Left = node1;
            Right = node2;
            Operation = new RawSqlNode(operation);

            Childs.AddRange(Left, Operation, Right);
        }

        public BinaryConditionNode(SqlNode node1, ComparerToken comparer, SqlNode node2)
        {
            Left = node1;
            Right = node2;
            Operation = comparer;

            Childs.AddRange(Left, Operation, Right);
        }
    }
}