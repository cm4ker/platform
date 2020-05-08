using System;
using System.Collections.Generic;

namespace Aquila.Compiler.Contracts
{
    public interface IConstructor : IEquatable<IConstructor>
    {
        bool IsPublic { get; }
        bool IsStatic { get; }
        IReadOnlyList<IParameter> Parameters { get; }
    }
}