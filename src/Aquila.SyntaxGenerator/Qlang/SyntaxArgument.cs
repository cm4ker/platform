using System.Xml.Serialization;

namespace Aquila.SyntaxGenerator.QLang
{
    public abstract class SyntaxArgument
    {
        [XmlAttribute] public string Name { get; set; }
        [XmlAttribute] public string Type { get; set; }

        [XmlAttribute] public bool DenyChildrenFill { get; set; }

        [XmlAttribute] public bool DenyDeclare { get; set; }

        [XmlAttribute] public bool PassBase { get; set; }
        public bool Null { get; set; }

        public bool IsNeedInitialize()
        {
            return false;
        }

        public bool IsPrimetive()
        {
            return false;
        }
    }
}