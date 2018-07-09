using ZenPlatform.Shared.Tree;

namespace ZenPlatform.QueryBuilder.Common
{
    public class LogicalNode : Node
    {
        public enum LogicalNodeType
        {
            And,
            Or
        }
    }
}