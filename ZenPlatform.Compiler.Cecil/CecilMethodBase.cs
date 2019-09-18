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

        public CecilMethodBase(CecilTypeSystem typeSystem, MethodReference method, MethodDefinition def,
            TypeReference declaringType)
        {
            TypeSystem = typeSystem;
            Reference = method;
            Definition = def;
            ContextResolver = new CecilContextResolver(typeSystem, method.Module);
            _declaringTR = declaringType;
        }

        public CecilMethodBase(CecilTypeSystem typeSystem, MethodReference method, TypeReference declaringType)
            : this(typeSystem, method, method.Resolve(), declaringType)
        {
        }

        private void UpdateReferenceInfo()
        {
        }

        protected CecilContextResolver ContextResolver { get; }

        public CecilTypeSystem TypeSystem { get; }

        public MethodDefinition Definition { get; }

        public MethodReference Reference { get; }

        public string Name => Definition.Name;

        public IType ReturnType => ContextResolver.GetType(Definition.ReturnType);
        public IType DeclaringType => ContextResolver.GetType(_declaringTR);

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