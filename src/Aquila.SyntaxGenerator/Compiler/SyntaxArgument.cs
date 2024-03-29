using System.Xml.Serialization;

namespace Aquila.SyntaxGenerator.Compiler
{
    public abstract class SyntaxArgument
    {
        [XmlAttribute] public string Name { get; set; }

        [XmlAttribute] public string Type { get; set; }

        [XmlAttribute] public bool DenyChildrenFill { get; set; }

        [XmlAttribute] public bool PassBase { get; set; }

        [XmlAttribute] public bool OnlyArgument { get; set; }

        [XmlAttribute] public bool ImplicitPassInChildren { get; set; }
    }
}