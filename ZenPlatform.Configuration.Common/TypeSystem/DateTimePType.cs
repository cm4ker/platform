using System;
using ZenPlatform.Configuration.Contracts.TypeSystem;

namespace ZenPlatform.Configuration.Common.TypeSystem
{
    public class DateTimePType : PType
    {
        public override uint SystemId => 3;

        public override Guid Id => new Guid(1, 2, 4, 56, 72, 234, 234, 23, 123, 12, 3);

        public override string Name
        {
            get { return "DateTime"; }
        }

        public override bool IsPrimitive => true;
        public override PrimitiveKind PrimitiveKind => PrimitiveKind.DateTime;
        
        internal DateTimePType(TypeManager ts) : base(ts)
        {
        }
    }
}