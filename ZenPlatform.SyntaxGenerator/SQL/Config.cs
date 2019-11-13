using System.Collections.Generic;
using System.Xml.Serialization;
using ZenPlatform.SyntaxGenerator.Compiler;

namespace ZenPlatform.SyntaxGenerator.SQL
{
    public class Config
    {
        /// <summary>
        /// The syntax collection
        /// </summary>
        [XmlElement("Syntax")]
        public List<SQLSyntax> Syntaxes { get; set; }
    }
}