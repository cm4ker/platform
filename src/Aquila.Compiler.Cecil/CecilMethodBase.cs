using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using Mono.Cecil;
using Aquila.Compiler.Contracts;

namespace Aquila.Compiler.Cecil
{
    internal abstract class CecilMethodBase
    {
        private TypeReference _declaringTR;

        public CecilMethodBase(CecilTypeSystem typeSystem, MethodReference method, MethodDefinition def,
            TypeReference declaringType)
        {
            TypeSystem = typeSystem;
            Reference = method;
            Definition = def ?? throw new NullReferenceException();
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


        private List<CecilParameter> _parameters;

        public IReadOnlyList<IParameter> Parameters =>
            _parameters ??= Definition.Parameters
                .Select(p => new CecilParameter(TypeSystem, Definition,
                    new ParameterDefinition(p.Name, p.Attributes, ContextResolver.Import(p.ParameterType))))
                .ToList();

        private IEmitter _generator;

        public IEmitter Generator => _generator ??= new CecilEmitter(TypeSystem, Definition);
    }
}