using System;
using System.Data;


namespace ZenPlatform.Configuration.ConfigurationLoader.Structure.Data.Types.Primitive
{
    public class XCGuid : XCPremitiveType
    {
        public override int Id => 4;

        public override Guid Guid => new Guid(1, 2, 4, 56, 72, 234, 234, 23, 123, 12, 4);
        
        public override string Name
        {
            get { return "Guid"; }
        }

        public override bool IsNullable { get; set; }
        public override int ColumnSize { get; set; }

        public override int Precision { get; set; }
        public override int Scale { get; set; }

        public override DbType DBType
        {
            get { return DbType.Guid; }
        }

        public override Type CLRType
        {
            get { return typeof(Guid); }
        }

    }
}