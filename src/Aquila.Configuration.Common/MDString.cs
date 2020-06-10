using System;
using Aquila.Compiler.Contracts;

namespace Aquila.Configuration.Common
{
    /// <summary>
    ///  Тип строки
    /// </summary>
    public sealed class MDString : MDPrimitive, IEquatable<MDString>
    {
        public override Guid Guid => TypeConstants.String;

        public MDString()
        {
        }


        public MDString(int size)
        {
            Size = size;
        }


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