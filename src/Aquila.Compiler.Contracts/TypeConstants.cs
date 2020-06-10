using System;
using System.Dynamic;
using System.Xml;

namespace Aquila.Compiler.Contracts
{
    public class TypeConstants
    {
        public static readonly (Guid guid, uint sysId)
            Binary = (new Guid(1, 2, 4, 56, 72, 234, 234, 23, 123, 12, 1), 1);

        public static readonly (Guid guid, uint sysId)
            Boolean = (new Guid(1, 2, 4, 56, 72, 234, 234, 23, 123, 12, 2), 2);

        public static readonly (Guid guid, uint sysId)
            DateTime = (new Guid(1, 2, 4, 56, 72, 234, 234, 23, 123, 12, 3), 3);

        public static readonly (Guid guid, uint sysId)
            Guid = (new Guid(1, 2, 4, 56, 72, 234, 234, 23, 123, 12, 4), 4);

        public static readonly (Guid guid, uint sysId)
            Decimal = (new Guid(1, 2, 4, 56, 72, 234, 234, 23, 123, 12, 5), 5);

        public static readonly (Guid guid, uint sysId) Numeric = Decimal;

        public static readonly (Guid guid, uint sysId)
            String = (new Guid(1, 2, 4, 56, 72, 234, 234, 23, 123, 12, 6), 6);

        public static readonly (Guid guid, uint sysId)
            Int = (new Guid(1, 2, 4, 56, 72, 234, 234, 23, 123, 12, 7), 7);

        public static readonly (Guid guid, uint sysId)
            Object = (new Guid(1, 2, 4, 56, 72, 234, 234, 23, 123, 12, 8), 8);

        public static readonly (Guid guid, uint sysId)
            Double = (new Guid(1, 2, 4, 56, 72, 234, 234, 23, 123, 12, 9), 9);

        public static readonly (Guid guid, uint sysId)
            Void = (new Guid(1, 2, 4, 56, 72, 234, 234, 23, 123, 12, 10), 10);


        public static Guid GetIdFromName(string fullName)
        {
            return fullName switch
            {
                "System.String" => String.guid,
                "System.Int" => Int.guid,
                "System.Object" => Object.guid,
                "System.Boolean" => Boolean.guid,
                "System.Decimal" => Decimal.guid,
                "System.Double" => Double.guid,
                "System.DateTime" => DateTime.guid,
                "System.Byte[]" => Binary.guid,
                "System.Guid" => Guid.guid,
                _ => System.Guid.NewGuid()
            };
        }
    }
}