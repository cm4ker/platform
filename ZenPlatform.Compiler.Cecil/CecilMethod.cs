using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Mono.Cecil;
using ZenPlatform.Compiler.Contracts;

namespace ZenPlatform.Compiler.Cecil
{
    class CecilMethodBase
    {
        public CecilTypeSystem TypeSystem { get; }
        public MethodReference Reference { get; }
        public MethodReference IlReference { get; }
        public MethodDefinition Definition { get; }
        private TypeReference _declaringTypeReference;


        public CecilMethodBase(CecilTypeSystem typeSystem, MethodDefinition method, TypeReference declaringType)
        {
            TypeSystem = typeSystem;

            MethodReference MakeRef(bool transform)
            {
                TypeReference Transform(TypeReference r) => transform ? r.TransformGeneric(declaringType) : r;

                var reference = new MethodReference(method.Name, Transform(method.ReturnType),
                    declaringType)
                {
                    HasThis = method.HasThis,
                    ExplicitThis = method.ExplicitThis,
                };
                foreach (ParameterDefinition parameter in method.Parameters)
                    reference.Parameters.Add(
                        new ParameterDefinition(Transform(parameter.ParameterType)));

                foreach (var genericParam in method.GenericParameters)
                    reference.GenericParameters.Add(new GenericParameter(genericParam.Name, reference));
                return reference;
            }

            Reference = MakeRef(true);
            IlReference = MakeRef(false);
            Definition = method;
            _declaringTypeReference = declaringType;
        }

        public string Name => Reference.Name;
        public bool IsPublic => Definition.IsPublic;
        public bool IsStatic => Definition.IsStatic;
        private IType _returnType;


        public IType ReturnType =>
            _returnType ?? (_returnType = TypeSystem.Resolve(Reference.ReturnType));

        private IType _declaringType;

        public IType DeclaringType =>
            _declaringType = _declaringType ?? (_declaringType = TypeSystem.Resolve(_declaringTypeReference));

        private IReadOnlyList<IType> _parameters;

        public IReadOnlyList<IType> Parameters =>
            _parameters ?? (_parameters =
                Reference.Parameters.Select(p => TypeSystem.Resolve(p.ParameterType)).ToList());

        private IEmitter _generator;

        public IEmitter Generator =>
            _generator ?? (_generator = new CecilEmitter(TypeSystem, Definition));
    }

    [DebuggerDisplay("{" + nameof(Reference) + "}")]
    class CecilMethod : CecilMethodBase, IMethodBuilder
    {
        public CecilMethod(CecilTypeSystem typeSystem, MethodDefinition methodDef,
            TypeReference declaringType) : base(typeSystem, methodDef, declaringType)
        {
        }

        public bool Equals(IMethod other) => other is CecilMethod cm
                                             && cm.Reference.Equals(Reference);
    }

    [DebuggerDisplay("{" + nameof(Reference) + "}")]
    class CecilConstructor : CecilMethodBase, IConstructorBuilder
    {
        public CecilConstructor(CecilTypeSystem typeSystem, MethodDefinition methodDef,
            TypeReference declaringType) : base(typeSystem, methodDef, declaringType)
        {
        }

        public bool Equals(IConstructor other) => other is CecilConstructor cm
                                                  && cm.Reference.Equals(Reference);
    }

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
        public IType ReturnType { get; } = PseudoType.Unknown;
        public IReadOnlyList<IType> Parameters { get; } = new IType[0];
        public IType DeclaringType { get; } = PseudoType.Unknown;
    }
}