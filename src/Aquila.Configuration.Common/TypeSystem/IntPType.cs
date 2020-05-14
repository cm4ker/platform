using System;
using Aquila.Configuration.Contracts.TypeSystem;

namespace Aquila.Configuration.Common.TypeSystem
{
    public class IntPType : PType
    {
        internal IntPType(ITypeManager ts) : base(ts)
        {
            BaseId = null;
        }

        public override uint SystemId => 7;

        public override Guid Id => new Guid(1, 2, 4, 56, 72, 234, 234, 23, 123, 12, 7);

        public override bool IsPrimitive => true;
        public override PrimitiveKind PrimitiveKind => PrimitiveKind.Int;
        
        public override string Name
        {
            get { return "Int"; }
        }
    }
}