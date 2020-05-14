using System;
using Aquila.Core.Contracts.TypeSystem;

namespace Aquila.Configuration.Common.TypeSystem
{
    public class GuidPType : PType
    {
        public override uint SystemId => 4;

        public override Guid Id => new Guid(1, 2, 4, 56, 72, 234, 234, 23, 123, 12, 4);

        public override string Name
        {
            get { return "Guid"; }
        }

        public override bool IsPrimitive => true;
        public override PrimitiveKind PrimitiveKind => PrimitiveKind.Guid;
        
        internal GuidPType(ITypeManager ts) : base(ts)
        {
        }
    }
}