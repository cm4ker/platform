using System;
using ConfCore;
using Type = ConfCore.Type;

namespace Aquila.Configuration.Structure.Data.Types.Primitive
{
    public class BooleanType : Type
    {
        internal BooleanType(TypeSystem ts) : base(ts)
        {
        }

        public override uint SystemId => 2;

        public override Guid Id => new Guid(1, 2, 4, 56, 72, 234, 234, 23, 123, 12, 2);

        public override string Name
        {
            get { return "Boolean"; }
        }
    }
}