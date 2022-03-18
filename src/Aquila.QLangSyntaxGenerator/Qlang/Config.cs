using System.Collections.Generic;
using System.Xml.Serialization;

namespace Aquila.SyntaxGenerator.QLang
{
    public class Config
    {
        /// <summary>
        /// The syntax collection
        /// </summary>
        [XmlElement("Syntax")]
        public List<QLangSyntax> Syntaxes { get; set; }
    }
}