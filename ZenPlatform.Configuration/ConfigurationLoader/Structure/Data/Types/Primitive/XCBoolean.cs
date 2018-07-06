using System;
using System.Data;


namespace ZenPlatform.Configuration.ConfigurationLoader.Structure.Data.Types.Primitive
{
    public class XCBoolean : XCPremitiveType
    {
        public override int Id => 2;

        public override Guid Guid => new Guid(1, 2, 4, 56, 72, 234, 234, 23, 123, 12, 2);

        public override string Name
        {
            get { return "Boolean"; }
        }

        public override bool IsNullable { get; set; }
        public override int ColumnSize { get; set; }
        public override DbType DBType
        {
            get { return DbType.Boolean; }
        }

        public override Type CLRType
        {
            get { return (IsNullable) ? typeof(bool?) : typeof(bool); }
        }
        public override int Precision { get; set; }
        public override int Scale { get; set; }

    }
}