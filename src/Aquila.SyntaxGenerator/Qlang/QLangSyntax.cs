using System.Collections.Generic;
using System.Xml.Serialization;
using Microsoft.CodeAnalysis;

namespace Aquila.SyntaxGenerator.QLang
{
    public class QLangSyntax
    {
        public QLangSyntax()
        {
            Arguments = new List<SyntaxArgument>();
            Base = "QItem";
        }

        /// <summary>
        /// The name of syntax node
        /// </summary>
        [XmlAttribute]
        public string Name { get; set; }

        /// <summary>
        /// Base of the current syntax by default it is AstNode
        /// </summary>
        [XmlAttribute]
        public string Base { get; set; }

        [XmlArrayItem("List", typeof(SyntaxArgumentList))]
        [XmlArrayItem("Single", typeof(SyntaxArgumentSingle))]
        public List<SyntaxArgument> Arguments { get; set; }

        /// <summary>
        /// Indicate that the syntax is abstract and only can be driven by another syntax
        /// </summary>
        [XmlAttribute]
        public bool IsAbstract { get; set; }

        /// <summary>
        /// Indicate that the syntax has scope
        /// </summary>
        [XmlAttribute]
        public bool IsScoped { get; set; }

        /// <summary>
        /// Indicate that the syntax relates to the endpoint symbol at the CST
        /// </summary>
        [XmlAttribute]
        public bool IsSymbol { get; set; }

        /// <summary>
        /// Namespace relative to root
        /// </summary>
        [XmlAttribute]
        public string NS { get; set; }

        /// <summary>
        /// Indicates that on generating syntax we not throw the Exceptions relates to the members
        /// </summary>
        [XmlAttribute]
        public bool NotThrowExMembersNotFound { get; set; }
    }
}