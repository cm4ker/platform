using System.Diagnostics;
using Mono.Cecil;
using Aquila.Compiler.Contracts;

namespace Aquila.Compiler.Cecil
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