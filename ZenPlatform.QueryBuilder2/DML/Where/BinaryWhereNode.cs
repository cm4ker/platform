using ZenPlatform.QueryBuilder.Common;
using ZenPlatform.Shared.Tree;

namespace ZenPlatform.QueryBuilder.DML.Where
{
    public class BinaryWhereNode : WhereExpression
    {
        public SqlNode Left { get; }
        public SqlNode Operation { get; }
        public SqlNode Right { get; }

        public BinaryWhereNode(string rawLeft, string operation, string rawRight)
        {
            Left = new RawSqlNode(rawLeft);
            Operation = new RawSqlNode(operation);
            Right = new RawSqlNode(rawRight);

            Childs.AddRange(Left, Operation, Right);
        }

        public BinaryWhereNode(SqlNode node1, string operation, SqlNode node2)
        {
            Left = node1;
            Right = node2;
            Operation = new RawSqlNode(operation);

            Childs.AddRange(Left, Operation, Right);
        }
    }
}