using System;
using ZenPlatform.Configuration.Contracts.TypeSystem;
using ZenPlatform.Configuration.Structure.Data.Types.Primitive;
using Type = ZenPlatform.Configuration.TypeSystem.Type;

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

        public static Guid GetTypeId(this MDType mdType, ITypeManager tm)
        {
            switch (mdType)
            {
                case MDString p:
                    var st = tm.FindType(mdType.Guid).GetSpec();
                    st.Size = p.Size;
                    tm.Register(st);
                    return st.Id;
                case MDNumeric n:
                    var nt = tm.FindType(mdType.Guid).GetSpec();
                    nt.Scale = n.Scale;
                    nt.Precision = n.Precision;
                    tm.Register(nt);
                    return nt.Id;
                case MDBinary b:
                    var bt = tm.FindType(mdType.Guid).GetSpec();
                    bt.Size = b.Size;
                    tm.Register(bt);
                    return bt.Id;

                default: return mdType.Guid;
            }
        }
    }
}