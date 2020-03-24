using System.Collections.Generic;
using System.Diagnostics;
using Mono.Cecil;
using ZenPlatform.Compiler.Contracts;

namespace ZenPlatform.Compiler.Cecil
{
    [DebuggerDisplay("{" + nameof(Definition) + "}")]
    class CecilConstructor : CecilMethodBase, IConstructorBuilder
    {
        private readonly MethodDefinition _methodDef;

        public CecilConstructor(CecilTypeSystem typeSystem, MethodDefinition methodDef, MethodReference reference,
            TypeReference declaringType) : base(typeSystem, reference, methodDef, declaringType)
        {
            _methodDef = methodDef;
        }

        public bool Equals(IConstructor other) => other is CecilConstructor cm
                                                  && cm.Definition.Equals(Definition);

        public bool IsPublic => _methodDef.IsPublic;
        public bool IsStatic => _methodDef.IsStatic;

        public IParameter DefineParameter(IType type)
        {
            var pd = new ParameterDefinition(ContextResolver.GetReference((ITypeReference) type));
            _methodDef.Parameters.Add(pd);

            var pp = new CecilParameter(TypeSystem, _methodDef, pd);
            ((List<CecilParameter>) Parameters).Add(pp);
            return pp;
        }
    }
}