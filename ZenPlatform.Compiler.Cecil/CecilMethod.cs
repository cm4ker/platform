using System.Diagnostics;
using System.Net.Http.Headers;
using Mono.Cecil;
using Mono.Cecil.Rocks;
using ZenPlatform.Compiler.Contracts;

namespace ZenPlatform.Compiler.Cecil
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
            var md = new MethodDefinition(Definition.Name, Definition.Attributes, Definition.ReturnType);

            GenericInstanceMethod gim = new GenericInstanceMethod(Definition);

            foreach (var type in typeArguments)
            {
                gim.GenericArguments.Add(_md.ImportReference(TypeSystem.GetTypeReference(type)));
            }

            return new CecilMethod(TypeSystem, gim, DeclaringTypeReference, _md);
        }
    }


    [DebuggerDisplay("{" + nameof(Definition) + "}")]
    internal class CecilMethodBuilder : CecilMethodBase, IMethodBuilder
    {
        private readonly MethodDefinition _methodDef;
        private readonly ModuleDefinition _md;

        public CecilMethodBuilder(CecilTypeSystem typeSystem, MethodDefinition methodDef,
            TypeReference declaringType, ModuleDefinition md) : base(typeSystem, methodDef, declaringType)
        {
            _methodDef = methodDef;
            _md = md;
        }

        public bool Equals(IMethod other)
        {
            return ((CecilMethodBase) other).Definition.Equals(Definition);
        }

        public IMethod MakeGenericMethod(IType[] typeArguments)
        {
            throw new System.NotImplementedException();
        }

        public IParameter DefineParameter(string name, IType type, bool isOut, bool isRef)
        {
            var param = new ParameterDefinition(name, ParameterAttributes.None,
                _md.ImportReference(TypeSystem.GetTypeReference(type)));

            _methodDef.Parameters.Add(param);

            return new CecilParameter(TypeSystem, _methodDef, param);
        }

        public IMethodBuilder WithReturnType(IType type)
        {
            _methodDef.ReturnType = ContextResolver.GetReference((ITypeReference) type);
            return this;
        }
    }
}