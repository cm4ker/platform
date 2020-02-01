using System;

namespace ZenPlatform.Configuration.Common
{
    /// <summary>
    ///  Тип строки
    /// </summary>
    public class MDString : MDPrimitive, IEquatable<MDString>
    {
        public override Guid Guid => new Guid(1, 2, 4, 56, 72, 234, 234, 23, 123, 12, 6);


        public override string Name
        {
            get { return "String"; }
        }

        public int Size { get; set; }

        public bool Equals(MDString other)
        {
            if (other == null) return false;

            return this.Guid == other.Guid &&
                   this.Size == other.Size;
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            return Equals(obj as MDString);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Guid, Size);
        }
    }
}