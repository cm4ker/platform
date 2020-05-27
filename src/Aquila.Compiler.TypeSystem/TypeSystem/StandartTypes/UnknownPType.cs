using System;
using Aquila.Core.Contracts.TypeSystem;

namespace Aquila.Compiler.Aqua.TypeSystem.StandartTypes
{
    public sealed class UnknownPType : PType
    {
        internal UnknownPType(TypeManager ts) : base(ts)
        {
            ts.AddOrUpdateSetting(new ObjectSetting {ObjectId = Id, SystemId = 0});
        }

        public override Guid Id => Guid.Empty;

        public override bool IsAbstract => false;
        public override bool IsPrimitive => true;

        public override PrimitiveKind PrimitiveKind => PrimitiveKind.Unknown;

        public override string Name
        {
            get { return "Unknown"; }
        }
    }
}