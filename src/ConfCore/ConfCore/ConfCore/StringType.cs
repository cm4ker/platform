using System;
using System.Data;
using System.Xml.Serialization;
using ConfCore;
using Type = ConfCore.Type;

namespace ZenPlatform.Configuration.Structure.Data.Types.Primitive
{
    /// <summary>
    ///  Тип строки
    /// </summary>
    public class StringType : Type
    {
        internal StringType(TypeSystem ts) : base(ts)
        {
        }

        public override uint SystemId => 6;

        public override Guid Id => new Guid(1, 2, 4, 56, 72, 234, 234, 23, 123, 12, 6);


        public override string Name
        {
            get { return "String"; }
        }
    }
}