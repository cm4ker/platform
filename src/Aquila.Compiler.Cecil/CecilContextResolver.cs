using Mono.Cecil;
using Aquila.Compiler.Contracts;

namespace Aquila.Compiler.Cecil
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

        public TypeReference Import(TypeReference tr)
        {
            if (tr.Scope.Name.Contains("System.Private.CoreLib"))
            {
                //In this case we need redirect type
                tr = new TypeReference(tr.Namespace, tr.Name, _moduleDef,
                    AssemblyNameReference.Parse(_ts.GetSystemBindings().MSCORLIB));
            }

            return _moduleDef.ImportReference(tr);
        }

        public TypeReference GetReference(ITypeReference type) => Import(type.Reference);

        public IType GetType(TypeReference tr) => _ts.Resolve(tr);
    }
}