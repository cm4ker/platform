using ZenPlatform.QueryBuilder.Common;
using ZenPlatform.Shared.Tree;

namespace ZenPlatform.QueryBuilder.DML.Where
{
    public class BinaryWhereNode : WhereExpression
    {
        public Node Left { get; }
        public Node Operation { get; }
        public Node Right { get; }

        public BinaryWhereNode(string rawLeft, string operation, string rawRight)
        {
            Left = new RawSqlNode(rawLeft);
            Operation = new RawSqlNode(operation);
            Right = new RawSqlNode(rawRight);

            Childs.AddRange(new[] {Left, Operation, Right});
        }

        public BinaryWhereNode(Node node1, string operation, Node node2)
        {
            Left = node1;
            Right = node2;
            Operation = new RawSqlNode(operation);

            Childs.AddRange(new[] {Left, Operation, Right});
        }
    }
}