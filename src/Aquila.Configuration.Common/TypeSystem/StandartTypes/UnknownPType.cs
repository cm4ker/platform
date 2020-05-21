using System;
using Aquila.Core.Contracts.TypeSystem;

namespace Aquila.Configuration.Common.TypeSystem
{
    public class UnknownPType : PType
    {
        internal UnknownPType(ITypeManager ts) : base(ts)
        {
        }


        public override uint SystemId => 0;

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