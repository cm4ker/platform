using ZenPlatform.Shared.Tree;

namespace ZenPlatform.QueryBuilder.Common
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