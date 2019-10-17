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

        public TypeReference GetReference(IType type) =>
            Import(_ts.GetTypeReference(type));

        public TypeReference Import(TypeReference tr) => _moduleDef.ImportReference(tr);

        public TypeReference GetReference(ITypeReference type) => Import(type.Reference);

        public IType GetType(TypeReference tr) => _ts.Resolve(tr);
    }
}