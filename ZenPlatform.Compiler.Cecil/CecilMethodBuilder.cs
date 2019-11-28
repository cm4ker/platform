using System.Collections.Generic;
using System.Diagnostics;
using Mono.Cecil;
using ZenPlatform.Compiler.Contracts;

namespace ZenPlatform.Compiler.Cecil
{
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

           
            var cecilParam = new CecilParameter(TypeSystem, _methodDef, param);
            ((List<CecilParameter>) this.Parameters).Add(cecilParam);
            
            _methodDef.Parameters.Add(param);
            
            return cecilParam;
        }

        public IMethodBuilder WithReturnType(IType type)
        {
            _methodDef.ReturnType = _md.ImportReference(((ITypeReference) type).Reference);
            return this;
        }
    }
}