using System;

namespace ZenPlatform.Configuration.TypeSystem
{
    public class BinaryType : Type
    {
        internal BinaryType(TypeSystem ts) : base(ts)
        {
        }

        public override uint SystemId => 1;

        public override Guid Id => new Guid(1, 2, 4, 56, 72, 234, 234, 23, 123, 12, 1);


        public override string Name
        {
            get { return "Binary"; }
        }
    }
}