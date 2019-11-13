using ZenPlatform.Configuration.Structure.Data.Types.Primitive;

namespace ZenPlatform.Configuration.Structure.Data.Types
{
    public static class PlatformTypes
    {
        public static XCBinary Binary = new XCBinary();
        public static XCDateTime DateTime = new XCDateTime();
        public static XCString String = new XCString();
        public static XCBoolean Boolean = new XCBoolean();
        public static XCNumeric Numeric = new XCNumeric();
        public static XCGuid Guid = new XCGuid();


        public static XCString GetString(int size) => new XCString {Size = size};
    }
}