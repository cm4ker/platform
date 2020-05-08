using Aquila.Shared.Tree;

namespace Aquila.QueryBuilder.Common
{
    public class LogicalNode : SqlNode
    {
        public enum LogicalNodeType
        {
            And,
            Or
        }
    }
}