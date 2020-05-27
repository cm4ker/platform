using System;
using Aquila.Compiler.Aqua.TypeSystem;
using Aquila.Compiler.Aqua.TypeSystem.StandartTypes;
using Aquila.Configuration.Common;
using Aquila.Configuration.Structure.Data.Types.Primitive;
using Aquila.Core.Contracts.TypeSystem;

namespace Aquila.Compiler.Aqua
{
    public static class TypeExtension
    {
        public static MDType GetMDType(this IPType type)
        {
            return type switch
            {
                GuidPType gt => new MDGuid(),
                IntPType it => new MDInt(),
                DateTimePType dt => new MDDateTime(),
                BooleanPType bt => new MDBoolean(),
                PTypeSpec ts => ts.BaseType switch
                {
                    NumericPType nt => new MDNumeric(ts.Scale, ts.Precision),
                    BinaryPType bt => new MDBinary(ts.Size),
                    StringPType st => new MDString(ts.Size),
                    _ => throw new NotSupportedException()
                },
                _ => new MDTypeRef(type.Id)
            };
        }
    }
}