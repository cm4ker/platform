using System.Collections.Generic;
using System.Xml.Serialization;

namespace ZenPlatform.SyntaxGenerator.SQL
{
    public class SQLSyntax
    {
        public SQLSyntax()
        {
            Arguments = new List<SyntaxArgument>();
            Base = "SSyntaxNode";
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


        [XmlAttribute] public bool Ddl { get; set; }

        [XmlArrayItem("List", typeof(SyntaxArgumentList))]
        [XmlArrayItem("Single", typeof(SyntaxArgumentSingle))]
        public List<SyntaxArgument> Arguments { get; set; }

        [XmlAttribute]
        public string NS { get; set; }
    }
}