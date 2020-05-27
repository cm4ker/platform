using System;
using Aquila.Core.Contracts.TypeSystem;

namespace Aquila.Compiler.Aqua.TypeSystem.StandartTypes
{
    public sealed class GuidPType : PType
    {
        internal GuidPType(TypeManager ts) : base(ts)
        {
            ts.AddOrUpdateSetting(new ObjectSetting {ObjectId = Id, SystemId = 4});
        }

        public override Guid Id => new Guid(1, 2, 4, 56, 72, 234, 234, 23, 123, 12, 4);

        public override string Name
        {
            get { return "Guid"; }
        }

        public override bool IsPrimitive => true;
        public override PrimitiveKind PrimitiveKind => PrimitiveKind.Guid;
    }
}