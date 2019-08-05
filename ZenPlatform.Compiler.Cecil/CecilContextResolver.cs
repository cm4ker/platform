using Mono.Cecil;
using ZenPlatform.Compiler.Contracts;

namespace ZenPlatform.Compiler.Cecil
{
    class CecilContextResolver
    {
        private readonly CecilTypeSystem _ts;
        private readonly ModuleDefinition _moduleDef;

        public CecilContextResolver(CecilTypeSystem ts, ModuleDefinition moduleDef)
        {
            _ts = ts;
            _moduleDef = moduleDef;
        }

        public TypeReference GetReference(ITypeReference type)
        {
            return _moduleDef.ImportReference(type.Reference);
        }

        public IType GetType(TypeReference tr) => _ts.Resolve(tr);
    }
}