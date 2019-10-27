using System;
using System.Data;

namespace ZenPlatform.Configuration.Structure.Data.Types.Primitive
{
    public class XCBoolean : XCPrimitiveType
    {
        public override uint Id => 2;

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


        private bool ShouldSerializePrecision()
        {
            return false;
        }
        
        private bool ShouldSerializeScale()
        {
            return false;
        }
        
        
        private bool ShouldSerializeColumnSize()
        {
            return false;
        }
    }
}