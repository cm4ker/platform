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


}