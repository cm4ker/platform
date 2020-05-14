using System.Runtime.Serialization;

namespace Aquila.WebServiceCore
{
    [DataContract]
    public class ComplexObject
    {
        [DataMember] public string StringProperty { get; set; }

        [DataMember] public int IntProperty { get; set; }
    }
}