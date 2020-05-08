using System.Collections.Generic;
using System.Xml.Serialization;
using Aquila.SyntaxGenerator.Compiler;

namespace Aquila.SyntaxGenerator.SQL
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