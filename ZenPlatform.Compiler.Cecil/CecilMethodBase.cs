using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using Mono.Cecil;
using ZenPlatform.Compiler.Contracts;

namespace ZenPlatform.Compiler.Cecil
{
    internal abstract class CecilMethodBase
    {
        private TypeReference _declaringTR;

        public CecilMethodBase(CecilTypeSystem typeSystem, MethodReference method, TypeReference declaringType)
        {
            TypeSystem = typeSystem;
            Reference = method;
            Definition = method.Resolve();
            _declaringTR = declaringType;
        }

        private void UpdateReferenceInfo()
        {
        }

        public CecilTypeSystem TypeSystem { get; }

        public MethodDefinition Definition { get; }

        public MethodReference Reference { get; }

        public string Name => Definition.Name;

        public IType ReturnType => TypeSystem.Resolve(Definition.ReturnType);
        public IType DeclaringType => TypeSystem.Resolve(_declaringTR);

        protected TypeReference DeclaringTypeReference => _declaringTR;

        public bool IsPublic => Definition.IsPublic;
        public bool IsStatic => Definition.IsStatic;

        public IReadOnlyList<IParameter> Parameters => Definition.Parameters
            .Select(p => new CecilParameter(TypeSystem, Definition, p))
            .ToList();


        private IEmitter _generator;
        public IEmitter Generator => _generator ??= new CecilEmitter(TypeSystem, Definition);
    }
}