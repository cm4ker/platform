using System;
using System.Data;
using ConfCore;
using Type = ConfCore.Type;

namespace Aquila.Configuration.Structure.Data.Types.Primitive
{
    public class NumericType : Type
    {
        public override uint SystemId => 5;

        public override Guid Id => new Guid(1, 2, 4, 56, 72, 234, 234, 23, 123, 12, 5);


        public override string Name
        {
            get { return "Numeric"; }
        }

        internal NumericType(TypeSystem ts) : base(ts)
        {
        }
    }
}