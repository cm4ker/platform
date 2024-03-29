using System.Xml.Serialization;
using Aquila.SyntaxGenerator.Compiler;

namespace Aquila.SyntaxGenerator
{
    public sealed class SyntaxArgumentSingle : SyntaxArgument
    {
        /// <summary>
        /// The default value of the argument. It will be passed to the constructor  
        /// </summary>
        [XmlAttribute]
        public string Default { get; set; }
    }
}