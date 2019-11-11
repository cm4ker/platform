using System;
using System.Collections.Generic;
using System.Text;
using ZenPlatform.Shared.Tree;

namespace ZenPlatform.Core.Language.QueryLanguage
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
