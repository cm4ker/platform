using System.Collections.Generic;
using ZenPlatform.Compiler.Contracts;

namespace ZenPlatform.Compiler.Cecil
{
    class UnresolvedMethod : IMethod
    {
        public UnresolvedMethod(string name)
        {
            Name = name;
        }

        public bool Equals(IMethod other) => other == this;

        public string Name { get; }
        public bool IsPublic { get; }
        public bool IsStatic { get; }
        public IType ReturnType { get; } = UnknownType.Unknown;
        public IReadOnlyList<IParameter> Parameters { get; } = new IParameter[0];
        public IType DeclaringType { get; } = UnknownType.Unknown;

        public IMethod MakeGenericMethod(IType[] typeArguments)
        {
            throw new System.NotImplementedException();
        }
    }
}