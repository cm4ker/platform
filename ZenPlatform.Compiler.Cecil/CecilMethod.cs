using System.Diagnostics;
using Mono.Cecil;
using ZenPlatform.Compiler.Contracts;

namespace ZenPlatform.Compiler.Cecil
{
    [DebuggerDisplay("{" + nameof(Definition) + "}")]
    class CecilMethod : CecilMethodBase, IMethod
    {
        private readonly MethodDefinition _methodDef;

        public CecilMethod(CecilTypeSystem typeSystem, MethodDefinition methodDef,
            TypeReference declaringType) : base(typeSystem, methodDef, declaringType)
        {
            _methodDef = methodDef;
        }

        public bool Equals(IMethod other) => other is CecilMethod cm
                                             && cm.Definition.Equals(Definition);

        public bool IsPublic => _methodDef.IsPublic;
        public bool IsStatic => _methodDef.IsStatic;
    }


    internal class CecilMethodBuilder : CecilMethodBase, IMethodBuilder
    {
        private readonly MethodDefinition _methodDef;

        public CecilMethodBuilder(CecilTypeSystem typeSystem, MethodDefinition methodDef,
            TypeReference declaringType) : base(typeSystem, methodDef, declaringType)
        {
            _methodDef = methodDef;
        }

        public bool Equals(IMethod other)
        {
            return ((CecilMethodBase) other).Definition.Equals(Definition);
        }

        public bool IsPublic => _methodDef.IsPublic;
        public bool IsStatic => _methodDef.IsStatic;

        public IMethodBuilder WithParameter(string name, IType type, bool isOut, bool isRef)
        {
            _methodDef.Parameters.Add(new ParameterDefinition(name, ParameterAttributes.None,
                TypeSystem.GetTypeReference(type)));

            return this;
        }

        public IMethodBuilder WithReturnType(IType type)
        {
            _methodDef.ReturnType = TypeSystem.GetTypeReference(type);
            return this;
        }
    }
}