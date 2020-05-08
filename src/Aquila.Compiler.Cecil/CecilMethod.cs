using System;
using System.Diagnostics;
using System.Net.Http.Headers;
using Mono.Cecil;
using Mono.Cecil.Rocks;
using Aquila.Compiler.Contracts;

namespace Aquila.Compiler.Cecil
{
    [DebuggerDisplay("{" + nameof(Definition) + "}")]
    class CecilMethod : CecilMethodBase, IMethod
    {
        private readonly ModuleDefinition _md;

        public CecilMethod(CecilTypeSystem typeSystem, MethodReference methodDef,
            TypeReference declaringType, ModuleDefinition md) : base(typeSystem, methodDef, declaringType)
        {
            _md = md;
        }

        public bool Equals(IMethod other) => other is CecilMethod cm
                                             && cm.Definition.Equals(Definition);

        public IMethod MakeGenericMethod(IType[] typeArguments)
        {
            //var md = new MethodDefinition(Definition.Name, Definition.Attributes, Definition.ReturnType);

            GenericInstanceMethod gim = new GenericInstanceMethod(Definition);

            foreach (var type in typeArguments)
            {
                gim.GenericArguments.Add(_md.ImportReference(TypeSystem.GetTypeReference(type)));
            }

            if (gim.Resolve() == null) throw new NullReferenceException();

            return new CecilMethod(TypeSystem, gim, DeclaringTypeReference, _md);
        }
    }
}