using System;
using Aquila.Compiler.Aqua.TypeSystem.Exported;
using Aquila.Compiler.Contracts;
using Aquila.Core.Contracts.TypeSystem;

namespace Aquila.Compiler.Aqua.TypeSystem.StandartTypes
{
    public sealed class NumericPType : PExportedType
    {
        internal NumericPType(TypeManager ts, IType type) : base(ts, type)
        {
            ts.AddOrUpdateSetting(new ObjectSetting {ObjectId = Id, SystemId = TypeConstants.Numeric.sysId});
        }

        public override Guid Id => TypeConstants.Numeric.guid;

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