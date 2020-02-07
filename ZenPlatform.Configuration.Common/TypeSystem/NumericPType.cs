using System;
using ZenPlatform.Configuration.Common.TypeSystem;
using ZenPlatform.Configuration.Contracts.TypeSystem;

namespace ZenPlatform.Configuration.TypeSystem
{
    public class NumericPType : PType
    {
        public override uint SystemId => 5;

        public override Guid Id => new Guid(1, 2, 4, 56, 72, 234, 234, 23, 123, 12, 5);


        public override string Name
        {
            get { return "Numeric"; }
        }

        public override bool IsPrimitive => true;
        
        public override PrimitiveKind PrimitiveKind => PrimitiveKind.Numeric;
        
        internal NumericPType(ITypeManager ts) : base(ts)
        {
        }
    }
}