using System.Collections.Generic;
using System.Xml.Serialization;
using ZenPlatform.SyntaxGenerator.Compiler;

namespace ZenPlatform.SyntaxGenerator.Compiler
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