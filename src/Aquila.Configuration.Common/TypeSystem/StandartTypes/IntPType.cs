using System;
using Aquila.Core.Contracts.TypeSystem;

namespace Aquila.Configuration.Common.TypeSystem.StandartTypes
{
    public sealed class IntPType : PType
    {
        internal IntPType(ITypeManager ts) : base(ts)
        {
            ts.AddOrUpdateSetting(new ObjectSetting {ObjectId = Id, SystemId = 7});
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