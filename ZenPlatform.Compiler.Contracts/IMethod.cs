using System;
using System.Collections.Generic;

namespace ZenPlatform.Compiler.Contracts
{
    public interface IMethod : IEquatable<IMethod>, IMember
    {
        bool IsPublic { get; }
        bool IsStatic { get; }
        IType ReturnType { get; }
        IReadOnlyList<IType> Parameters { get; }
        IType DeclaringType { get; }
    }
}