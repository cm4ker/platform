using System.Collections.Generic;
using System.Xml.Serialization;

namespace Aquila.SyntaxGenerator.BoundTree
{
    public class Config
    {
        /// <summary>
        /// The syntax collection
        /// </summary>
        [XmlElement("Syntax")]
        public List<CompilerSyntax> Syntaxes { get; set; }
    }
}