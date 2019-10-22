using System;
using System.Collections.Generic;
using System.IO;

namespace ZenPlatform.Compiler.Contracts
{
    public interface IAssembly : IEquatable<IAssembly>
    {
        string Name { get; }
        IReadOnlyList<ICustomAttribute> CustomAttributes { get; }

        IType FindType(string fullName);

        void Write(string fileName);
        void Write(Stream stream);

        ITypeSystem TypeSystem { get; }
    }

    public static class AssemblyExtension
    {
        public static byte[] ToBytes(this IAssembly assembly)
        {
            using (var stream = new MemoryStream())
            {
                assembly.Write(stream);
                return stream.ToArray();
            }
        }
    }
}