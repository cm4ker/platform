using System;

namespace ConfCore
{
    public class IntType : Type
    {
        internal IntType(TypeSystem ts) : base(ts)
        {
            ParentId = null;
        }

        public override uint SystemId => 7;

        public override Guid Id => new Guid(1, 2, 4, 56, 72, 234, 234, 23, 123, 12, 7);

        public override string Name
        {
            get { return "Int"; }
        }
    }
}