using System;
using ZenPlatform.Configuration.Contracts.TypeSystem;

namespace ZenPlatform.Configuration.TypeSystem
{
    public class IntType : Type
    {
        internal IntType(ITypeManager ts) : base(ts)
        {
            BaseId = null;
        }

        public override uint SystemId => 7;

        public override Guid Id => new Guid(1, 2, 4, 56, 72, 234, 234, 23, 123, 12, 7);

        public override string Name
        {
            get { return "Int"; }
        }
    }
}