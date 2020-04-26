using System.Diagnostics;
using Mono.Cecil;
using ZenPlatform.Compiler.Contracts;

namespace ZenPlatform.Compiler.Cecil
{
    [DebuggerDisplay("{" + nameof(Reference) + "}")]
    class UnresolvedCecilType : UnknownType, ITypeReference
    {
        public TypeReference Reference { get; }

        public UnresolvedCecilType(TypeReference reference) : base("Unresolved:" + reference.FullName)
        {
            Reference = reference;
        }
    }
}