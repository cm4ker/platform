using System.Collections.Generic;
using System.Xml.Serialization;
using Aquila.SyntaxGenerator.Compiler;

namespace Aquila.SyntaxGenerator.Compiler
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