using System.Collections.Generic;
using System.Xml.Serialization;

namespace Aquila.SyntaxGenerator.BoundTree
{
    public class CompilerSyntax
    {
        public CompilerSyntax()
        {
            Arguments = new List<SyntaxArgument>();
            //Base = "LangElement";
        }

        /// <summary>
        /// Is syntax is list?
        /// </summary>
        [XmlAttribute]
        public bool IsList { get; set; }

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


        [XmlAttribute] public string Interface { get; set; }

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

        [XmlAttribute]
        public string OperationKind { get; set; }
    }
}