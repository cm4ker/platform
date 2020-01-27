using System;

namespace ZenPlatform.Configuration.TypeSystem
{
    public class DateTimeType : Type
    {
        public override uint SystemId => 3;

        public override Guid Id => new Guid(1, 2, 4, 56, 72, 234, 234, 23, 123, 12, 3);

        public override string Name
        {
            get { return "DateTime"; }
        }

        internal DateTimeType(TypeManager ts) : base(ts)
        {
        }
    }
}