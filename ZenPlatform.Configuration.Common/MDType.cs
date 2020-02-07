using System;
using System.Linq;
using System.Collections.Generic;
using ZenPlatform.Configuration.TypeSystem;
using ZenPlatform.Configuration.Contracts.TypeSystem;
using ZenPlatform.Configuration.Structure.Data.Types.Primitive;
using ZenPlatform.Configuration.Common.TypeSystem;

namespace ZenPlatform.Configuration.Common
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
        public static MDType GetMDType(this IType type)
        {
            return type switch
            {

                GuidType gt => new MDGuid(),
                IntType it => new MDInt(),
                DateTimeType dt => new MDDateTime(),
                BooleanType bt => new MDBoolean(),
                TypeSpec ts => ts.BaseType switch
                {
                    NumericType nt => new MDNumeric(ts.Scale, ts.Precision),
                    BinaryType bt => new MDBinary(ts.Size),
                    StringType st => new MDString(ts.Size),
                    _ => throw new NotSupportedException()
                },
                _ => new TypeRef(type.Id)
            };

        }
    }


}