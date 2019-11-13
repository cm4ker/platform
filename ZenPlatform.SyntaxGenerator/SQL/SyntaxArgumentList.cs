using System.Xml.Serialization;

namespace ZenPlatform.SyntaxGenerator.SQL
{
    public sealed class SyntaxArgumentList : SyntaxArgument
    {
        private string _type;
        [XmlAttribute]
        public override string Type {
            get
            {
                return $"List<{_type}>";
            }
            set
            {
                _type = value;
            }
        }
    }
}