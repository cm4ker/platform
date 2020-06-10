using System;
using Aquila.Compiler.Aqua.TypeSystem.Exported;
using Aquila.Compiler.Contracts;
using Aquila.Core.Contracts.TypeSystem;

namespace Aquila.Compiler.Aqua.TypeSystem.StandartTypes
{
    public sealed class DateTimePType : PExportedType
    {
        internal DateTimePType(TypeManager ts, IType type) : base(ts, type)
        {
            ts.AddOrUpdateSetting(new ObjectSetting {ObjectId = Id, SystemId = TypeConstants.DateTime.sysId});
        }

        public override Guid Id => TypeConstants.DateTime.guid;

        public override string Name
        {
            get { return "DateTime"; }
        }

        public override bool IsPrimitive => true;
        public override PrimitiveKind PrimitiveKind => PrimitiveKind.DateTime;
    }
}