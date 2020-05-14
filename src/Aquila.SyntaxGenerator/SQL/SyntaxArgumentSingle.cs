using System.Xml.Serialization;

namespace Aquila.SyntaxGenerator.SQL
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