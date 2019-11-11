using System;
using System.Data;
using System.Xml.Serialization;

namespace ZenPlatform.Configuration.Structure.Data.Types.Primitive
{
    /// <summary>
    /// ��� ������� ������
    /// </summary>
    public class XCString : XCPrimitiveType, IEquatable<XCString>
    {
        public override uint Id => 6;

        public override Guid Guid => new Guid(1, 2, 4, 56, 72, 234, 234, 23, 123, 12, 6);

        
        public override string Name
        {
            get { return "String"; }
        }

        public int Size { get; set; }

        public bool Equals(XCString other)
        {
            if (other == null) return false;

            return this.Guid == other.Guid && this.IsNullable == other.IsNullable &&
                this.Size == other.Size;
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            return Equals(obj as XCBinary);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Guid, IsNullable, Size);
        }

    }
}