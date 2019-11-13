using System.Linq;
using System.Xml.Serialization;

namespace ZenPlatform.SyntaxGenerator.SQL
{
    public abstract class SyntaxArgument
    {
        [XmlAttribute] public string Name { get; set; }
        [XmlAttribute] public virtual string Type { get; set; }
        [XmlAttribute] public bool Null { get; set; }
        [XmlAttribute] public bool Base { get; set; }
        public bool IsPrimetive()
        {
            return new string[] { "int", "string", "float", "bool", "JoinType", "SystemMethods", "OrderDirection" }.Contains(Type);
        }
        public bool IsNeedInitialize()
        {

            return !Base && !Null && !IsNeedCreate();
        }


        public bool IsNeedCreate()
        {

            return false;
        }
    }
}