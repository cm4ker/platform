using System.Diagnostics;
using Mono.Cecil;
using ZenPlatform.Compiler.Contracts;

namespace ZenPlatform.Compiler.Cecil
{
    [DebuggerDisplay("{" + nameof(Definition) + "}")]
    class CecilConstructor : CecilMethodBase, IConstructorBuilder
    {
        private readonly MethodDefinition _methodDef;

        public CecilConstructor(CecilTypeSystem typeSystem, MethodDefinition methodDef,
            TypeReference declaringType) : base(typeSystem, methodDef, declaringType)
        {
            _methodDef = methodDef;
        }

        public bool Equals(IConstructor other) => other is CecilConstructor cm
                                                  && cm.Definition.Equals(Definition);

        public bool IsPublic => _methodDef.IsPublic;
        public bool IsStatic => _methodDef.IsStatic;
    }
}