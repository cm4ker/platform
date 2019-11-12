using System.Xml.Serialization;
using ZenPlatform.SyntaxGenerator.Compiler;

namespace ZenPlatform.SyntaxGenerator
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