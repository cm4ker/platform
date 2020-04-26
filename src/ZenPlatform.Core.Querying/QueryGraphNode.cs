using ZenPlatform.Shared.Tree;

namespace ZenPlatform.Core.Querying
{
    public class QueryGraphNode : Node
    {
        public string Content { get; set; }

        /// <summary>
        /// Тип ноды
        /// </summary>
        public NodeType NodeType { get; set; }
    }


    public enum NodeType
    {
        Component,
        Property,
        Alias,
        Function
    }
}
