using System;
using System.Data;

namespace ZenPlatform.Configuration.Structure.Data.Types.Primitive
{
    /// <summary>
    /// Тип обычной строки
    /// </summary>
    public class XCString : XCPremitiveType
    {
        public override uint Id => 6;

        public override Guid Guid => new Guid(1, 2, 4, 56, 72, 234, 234, 23, 123, 12, 6);

        public override string Name
        {
            get { return "String"; }
        }

        public override bool IsNullable { get; set; }
        public override int ColumnSize { get; set; }

        public override int Precision { get; set; }
        public override int Scale { get; set; }

        public override DbType DBType
        {
            get { return DbType.String; }
        }

        public override Type CLRType
        {
            get { return typeof(string); }
        }

    }
}