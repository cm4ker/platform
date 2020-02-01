using System;
using ZenPlatform.Configuration.Structure.Data.Types.Primitive;

namespace ZenPlatform.Configuration.Common
{
    public static class MDTypes
    {
        public static MDType Int = new MDInt();
        public static MDType Binary = new MDBinary();
        public static MDType Guid = new MDGuid();
        public static MDType Boolean = new MDBoolean();
        public static MDType DateTime = new MDDateTime();

        public static MDType String(int size) => new MDString(size);
        public static MDType Numeric(int scale, int precision) => new MDNumeric(scale, precision);
        public static MDType Ref(Guid id) => new TypeRef(id);
    }
}