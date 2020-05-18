using System;
using System.Linq;
using System.Collections.Generic;
using Aquila.Configuration.Structure.Data.Types.Primitive;
using Aquila.Configuration.Common.TypeSystem;
using Aquila.Core.Contracts.TypeSystem;

namespace Aquila.Configuration.Common
{
    public class MDType
    {
        public virtual Guid Guid { get; set; }

        public virtual string Name { get; }

        protected virtual bool ShouldSerializeDescription()
        {
            return false;
        }

        protected virtual bool ShouldSerializeName()
        {
            return false;
        }

        protected virtual bool ShouldSerializeId()
        {
            return false;
        }
        
    }


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