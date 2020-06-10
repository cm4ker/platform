using System;
using Aquila.Compiler.Aqua.TypeSystem.Exported;
using Aquila.Compiler.Contracts;
using Aquila.Core.Contracts.TypeSystem;

namespace Aquila.Compiler.Aqua.TypeSystem.StandartTypes
{
    public sealed class BooleanPType : PExportedType
    {
        internal BooleanPType(TypeManager ts, IType type) : base(ts, type)
        {
            ts.AddOrUpdateSetting(new ObjectSetting {ObjectId = Id, SystemId = TypeConstants.Boolean.sysId});
        }

        public override Guid Id => TypeConstants.Boolean.guid;

        public override bool IsPrimitive => true;
        public override PrimitiveKind PrimitiveKind => PrimitiveKind.Boolean;

        public override string Name
        {
            get { return "Boolean"; }
        }
    }
}