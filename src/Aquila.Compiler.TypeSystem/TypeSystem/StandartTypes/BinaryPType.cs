using System;
using Aquila.Core.Contracts.TypeSystem;

namespace Aquila.Compiler.Aqua.TypeSystem.StandartTypes
{
    public sealed class BinaryPType : PType
    {
        internal BinaryPType(TypeManager ts) : base(ts)
        {
            ts.AddOrUpdateSetting(new ObjectSetting {ObjectId = Id, SystemId = 1});
        }

        public override Guid Id => new Guid(1, 2, 4, 56, 72, 234, 234, 23, 123, 12, 1);

        public override bool IsAbstract => true;
        public override bool IsPrimitive => true;
        public override PrimitiveKind PrimitiveKind => PrimitiveKind.Binary;

        public override string Name
        {
            get { return "Binary"; }
        }
    }
}