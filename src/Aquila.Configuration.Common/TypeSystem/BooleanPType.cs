using System;
using Aquila.Configuration.Contracts.TypeSystem;

namespace Aquila.Configuration.Common.TypeSystem
{
    public class BooleanPType : PType
    {
        internal BooleanPType(ITypeManager ts) : base(ts)
        {
        }

        public override uint SystemId => 2;

        public override Guid Id => new Guid(1, 2, 4, 56, 72, 234, 234, 23, 123, 12, 2);

        public override bool IsPrimitive => true;
        public override PrimitiveKind PrimitiveKind => PrimitiveKind.Boolean;
        
        public override string Name
        {
            get { return "Boolean"; }
        }
    }
}