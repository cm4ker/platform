using System;
using Aquila.Configuration.Structure.Data.Types.Primitive;
using Aquila.Core.Contracts.TypeSystem;

namespace Aquila.Configuration.Common
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
        public static MDType Ref(Guid id) => new MDTypeRef(id);

        public static Guid GetTypeId(this MDType mdType, ITypeManager tm, bool isArray = false)
        {
            Guid result = mdType.Guid;

            // switch (mdType)
            // {
            //     case MDString p:
            //         var st = tm.DefineType(mdType.Guid);
            //         st.SetSize(p.Size);
            //         tm.Register(st);
            //         result = st.Id;
            //         break;
            //
            //     case MDNumeric n:
            //         var nt = tm.DefineType(mdType.Guid);
            //         nt.SetScale(n.Scale);
            //         nt.SetPrecision(n.Precision);
            //         tm.Register(nt);
            //         result = nt.Id;
            //         break;
            //     case MDBinary b:
            //         var bt = tm.DefineType(mdType.Guid);
            //         bt.SetSize(b.Size);
            //         tm.Register(bt);
            //         result = bt.Id;
            //         break;
            // }
            //
            // if (isArray)
            // {
            //     var spec = tm.DefineType(result);
            //     spec.SetIsArray(true);
            //     tm.Register(spec);
            //     result = spec.Id;
            // }

            return result;
        }
    }
}