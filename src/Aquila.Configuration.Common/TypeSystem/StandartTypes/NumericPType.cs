using System;
using Aquila.Core.Contracts.TypeSystem;

namespace Aquila.Configuration.Common.TypeSystem.StandartTypes
{
    public sealed class NumericPType : PType
    {
        internal NumericPType(TypeManager ts) : base(ts)
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