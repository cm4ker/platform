using System;

namespace ZenPlatform.Configuration.TypeSystem
{
    public class GuidType : Type
    {
        public override uint SystemId => 4;

        public override Guid Id => new Guid(1, 2, 4, 56, 72, 234, 234, 23, 123, 12, 4);

        public override string Name
        {
            get { return "Guid"; }
        }

        internal GuidType(TypeSystem ts) : base(ts)
        {
        }
    }
}