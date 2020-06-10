using System;
using Aquila.Compiler.Aqua.TypeSystem.Exported;
using Aquila.Compiler.Contracts;
using Aquila.Core.Contracts.TypeSystem;

namespace Aquila.Compiler.Aqua.TypeSystem.StandartTypes
{
    public sealed class BinaryPType : PExportedType
    {
        internal BinaryPType(TypeManager ts, IType type) : base(ts, type)
        {
            ts.AddOrUpdateSetting(new ObjectSetting {ObjectId = Id, SystemId = TypeConstants.Binary.sysId});
        }

        public override Guid Id => TypeConstants.Binary.guid;

        public override bool IsAbstract => true;
        public override bool IsPrimitive => true;
        public override PrimitiveKind PrimitiveKind => PrimitiveKind.Binary;

        public override string Name
        {
            get { return "Binary"; }
        }
    }
}