using dnlib.DotNet;

namespace ZenPlatform.Compiler.Roslyn.DnlibBackend
{
    public class SreMetadataResolver : IResolver
    {
        private Resolver _resolver;

        public SreMetadataResolver(IAssemblyResolver asmResolver)
        {
            _resolver = new Resolver(asmResolver);
        }

        public TypeDef Resolve(TypeRef typeRef, ModuleDef sourceModule)
        {
            return _resolver.Resolve(typeRef, sourceModule);
        }

        public IMemberForwarded Resolve(MemberRef memberRef)
        {
            return _resolver.Resolve(memberRef);
        }
    }
}