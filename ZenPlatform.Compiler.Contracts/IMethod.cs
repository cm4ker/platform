using System;
using System.Collections.Generic;

namespace ZenPlatform.Compiler.Contracts
{
    public interface IMethod : IEquatable<IMethod>, IMember
    {
        bool IsPublic { get; }
        bool IsStatic { get; }
        IType ReturnType { get; }
        IReadOnlyList<IParameter> Parameters { get; }
        IType DeclaringType { get; }

        IMethod MakeGenericMethod(params IType[] typeArguments);
    }
}