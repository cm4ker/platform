using System;
using System.Collections.Generic;

namespace ZenPlatform.Compiler.Contracts
{
    public interface ICustomAttribute : IEquatable<ICustomAttribute>
    {
        IType Type { get; }
        List<object> Parameters { get; }
        Dictionary<string, object> Properties { get; }
    }
}