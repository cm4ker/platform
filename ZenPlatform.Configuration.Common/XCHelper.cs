using System;
using System.Linq;
using ZenPlatform.Configuration.Contracts;
using ZenPlatform.Configuration.Structure.Data.Types.Primitive;
using ZenPlatform.Shared.ParenChildCollection;

namespace ZenPlatform.Configuration.Structure.Helper
{
    public static class XCHelper
    {
        public static string ConvertToDbType(this IXCType type)
        {
            if (type is IXCPrimitiveType)
            {
                if (type is XCBinary b) return $"varbinary{b.Size}";
                if (type is XCGuid) return "guid";
                if (type is XCInt) return "int";
                if (type is XCNumeric n) return $"numeric({n.Scale}, {n.Precision})";
                if (type is XCDateTime) return "datetime";
                if (type is XCBoolean) return "bool";
                if (type is XCString s) return $"varchar({s.Size})";
            }

            if (type is IXCObjectType) return "guid";

            throw new Exception("Unknown type");
        }


    }
}