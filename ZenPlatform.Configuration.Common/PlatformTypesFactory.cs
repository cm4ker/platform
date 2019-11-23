using ZenPlatform.Configuration.Structure.Data.Types.Primitive;

namespace ZenPlatform.Configuration.Structure.Data.Types
{
    /// <summary>
    /// Фабрика типов платформы
    /// </summary>
    public static class PlatformTypesFactory
    {
        public static XCBoolean Boolean = new XCBoolean();
        public static XCGuid Guid = new XCGuid();
        public static XCInt Int = new XCInt();

        public static XCString GetString(int size) => new XCString {Size = size};

        public static XCNumeric GetNumeric(int scale, int precision) =>
            new XCNumeric() {Scale = scale, Precision = precision};
    }
}