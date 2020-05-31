using System;
using Aquila.Compiler.Aqua.TypeSystem.Exported;
using Aquila.Compiler.Contracts;
using Aquila.Core.Contracts.TypeSystem;

namespace Aquila.Compiler.Aqua.TypeSystem.StandartTypes
{
    public sealed class NumericPType : PExportType
    {
        internal NumericPType(TypeManager ts, IType type) : base(ts, type)
        {
            ts.AddOrUpdateSetting(new ObjectSetting {ObjectId = Id, SystemId = 5});
        }

        public override Guid Id => new Guid(1, 2, 4, 56, 72, 234, 234, 23, 123, 12, 5);

        public override string Name
        {
            get { return "Numeric"; }
        }

        public override bool IsPrimitive => true;

        public override bool IsAbstract => true;

        public override PrimitiveKind PrimitiveKind => PrimitiveKind.Numeric;

        public override bool IsScalePrecision => true;
    }
}