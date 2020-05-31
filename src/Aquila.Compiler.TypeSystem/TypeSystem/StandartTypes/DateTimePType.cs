using System;
using Aquila.Compiler.Aqua.TypeSystem.Exported;
using Aquila.Compiler.Contracts;
using Aquila.Core.Contracts.TypeSystem;

namespace Aquila.Compiler.Aqua.TypeSystem.StandartTypes
{
    public sealed class DateTimePType : PExportType
    {
        internal DateTimePType(TypeManager ts, IType type) : base(ts, type)
        {
            ts.AddOrUpdateSetting(new ObjectSetting {ObjectId = Id, SystemId = 3});
        }

        public override Guid Id => new Guid(1, 2, 4, 56, 72, 234, 234, 23, 123, 12, 3);

        public override string Name
        {
            get { return "DateTime"; }
        }

        public override bool IsPrimitive => true;
        public override PrimitiveKind PrimitiveKind => PrimitiveKind.DateTime;
    }
}